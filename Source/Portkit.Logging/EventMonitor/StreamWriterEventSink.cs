using System;
using System.Collections.Concurrent;
using System.Diagnostics.Tracing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Portkit.Logging.EventMonitor
{
    /// <summary>
    ///     A sink that writes to a stream.
    /// </summary>
    /// <remarks>This class is thread-safe.</remarks>
    public class StreamWriterEventSink : IObserver<string>, IDisposable
    {
        #region Fields

        private readonly Task _asyncProcessorTask;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly object _flushLockObject = new object();
        private readonly bool _isAsync;
        private readonly object _lockObject = new object();
        private readonly BlockingCollection<string> _pendingEntries;
        private readonly StreamWriter _writer;
        private bool _disposed;
        private volatile TaskCompletionSource<bool> _flushSource = new TaskCompletionSource<bool>();

        #endregion

        /// <summary>
        /// Creates a new instance of the <see cref="StreamWriterEventSink"/> class.
        /// </summary>
        /// <param name="writer"><see cref="StreamWriter"/> writer.</param>
        /// <param name="isAsync">Sets the way events are written to the stream writer.</param>
        public StreamWriterEventSink(StreamWriter writer, bool isAsync)
        {
            _writer = writer;
            _isAsync = isAsync;

            _flushSource.SetResult(true);

            if (isAsync)
            {
                _cancellationTokenSource = new CancellationTokenSource();
                _pendingEntries = new BlockingCollection<string>();
                _asyncProcessorTask = Task.Factory.StartNew(WriteEntriesAsync, TaskCreationOptions.LongRunning);
            }
        }

        #region IObserver Members

        /// <summary>
        ///     Notifies the observer that the provider has finished sending push-based notifications.
        /// </summary>
        public void OnCompleted()
        {
            FlushAsync().Wait();
            Dispose();
        }

        /// <summary>
        ///     Notifies the observer that the provider has experienced an error condition.
        /// </summary>
        /// <param name="error">An object that provides additional information about the error.</param>
        public void OnError(Exception error)
        {
            FlushAsync().Wait();
            Dispose();
        }

        /// <summary>
        ///     Provides the sink with new data to write.
        /// </summary>
        /// <param name="value">The current entry to write to the file.</param>
        public void OnNext(string value)
        {
            if (value == null)
            {
                return;
            }
            if (_isAsync)
            {
                _pendingEntries.Add(value);

                if (_flushSource.Task.IsCompleted)
                {
                    lock (_flushLockObject)
                    {
                        if (_flushSource.Task.IsCompleted)
                        {
                            _flushSource = new TaskCompletionSource<bool>();
                        }
                    }
                }
            }
            else
            {
                WriteSynchronously(value);
            }
        }

        #endregion

        /// <summary>
        ///     Flushes the buffer content to the file.
        /// </summary>
        /// <returns>The Task that gets completed when the buffer is flushed.</returns>
        public Task FlushAsync()
        {
            lock (_flushLockObject)
            {
                return _flushSource.Task;
            }
        }

        private void WriteSynchronously(string entry)
        {
            try
            {
                lock (_lockObject)
                {
                    _writer.Write(entry);
                    _writer.Flush();
                }
            }
            catch (Exception)
            {
                // Write failed.
            }
        }

        private void WriteEntriesAsync()
        {
            string entry;
            CancellationToken token = _cancellationTokenSource.Token;

            while (!token.IsCancellationRequested)
            {
                try
                {
                    if (_pendingEntries.Count == 0 && !_flushSource.Task.IsCompleted && !token.IsCancellationRequested)
                    {
                        _writer.Flush();
                        lock (_flushLockObject)
                        {
                            if (_pendingEntries.Count == 0 && !_flushSource.Task.IsCompleted &&
                                !token.IsCancellationRequested)
                            {
                                _flushSource.TrySetResult(true);
                            }
                        }
                    }

                    entry = _pendingEntries.Take(token);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (ObjectDisposedException)
                {
                    break;
                }

                try
                {
                    _writer.Write(entry);
                }
                catch (Exception)
                {
                    // Writer failed.
                }
            }

            lock (_flushLockObject)
            {
                _flushSource.TrySetResult(true);
            }
        }

        /// <summary>
        /// Factory method to create an <see cref="EventListener"/>.
        /// </summary>
        /// <param name="writer">Writer object</param>
        /// <param name="formatter">Content formatter</param>
        /// <param name="isAsync">Way of storing events.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public static EventListener CreateListener(StreamWriter writer, ContentFormatter formatter,
            bool isAsync = false)
        {
            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }
            if (formatter == null)
            {
                throw new ArgumentNullException("formatter");
            }

            var sink = new StreamWriterEventSink(writer, isAsync);
            var listener = new ObservableEventListener();
            listener.CreateSubscription(sink, entry => entry.ToString(formatter));

            return listener;
        }

        #region IDisposable Members

        /// <summary>
        ///     Releases all resources used by the current instance of the <see cref="StreamWriterEventSink" /> class.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <param name="disposing">A value indicating whether or not the class is disposing.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
            {
                return;
            }
            if (_disposed)
            {
                return;
            }
            lock (_lockObject)
            {
                if (!_disposed)
                {
                    _disposed = true;

                    if (_isAsync)
                    {
                        _cancellationTokenSource.Cancel();
                        _asyncProcessorTask.Wait();
                        _pendingEntries.Dispose();
                        _cancellationTokenSource.Dispose();
                    }
                    _writer.Dispose();
                }
            }
        }

        /// <summary>
        ///     Finalizes an instance of the <see cref="StreamWriterEventSink" /> class.
        /// </summary>
        ~StreamWriterEventSink()
        {
            Dispose(false);
        }

        #endregion
    }
}