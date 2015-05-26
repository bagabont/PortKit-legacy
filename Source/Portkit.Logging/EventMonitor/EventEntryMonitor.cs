using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Portkit.Logging.EventMonitor
{
    internal sealed class EventEntryMonitor : IObservable<EventEntry>, IObserver<EventEntry>, IDisposable
    {
        #region Nested classes

        private sealed class EmptyDisposable : IDisposable
        {
            public void Dispose()
            {
            }
        }

        private sealed class Unsubscriber : IDisposable
        {
            private IObserver<EventEntry> _observer;
            private EventEntryMonitor _subject;

            public Unsubscriber(EventEntryMonitor subject, IObserver<EventEntry> observer)
            {
                _subject = subject;
                _observer = observer;
            }

            public void Dispose()
            {
                IObserver<EventEntry> current = Interlocked.Exchange(ref _observer, null);
                if (current != null)
                {
                    _subject.Unsubscribe(current);
                    _subject = null;
                }
            }
        }

        #endregion

        #region Fields

        private readonly object _syncLock = new object();

        private volatile bool _isLocked;

        private volatile ReadOnlyCollection<IObserver<EventEntry>> _observers =
            new List<IObserver<EventEntry>>().AsReadOnly();

        #endregion

        #region IObservable Memebrs

        /// <summary>
        ///     Notifies the provider that an observer is to receive notifications.
        /// </summary>
        /// <returns>
        ///     A reference to an interface that allows observers to stop receiving notifications before the provider has finished
        ///     sending them.
        /// </returns>
        /// <param name="observer">The object that is to receive notifications.</param>
        public IDisposable Subscribe(IObserver<EventEntry> observer)
        {
            if (observer == null)
            {
                throw new ArgumentNullException("observer");
            }

            lock (_syncLock)
            {
                if (!_isLocked)
                {
                    List<IObserver<EventEntry>> copy = _observers.ToList();
                    copy.Add(observer);
                    _observers = copy.AsReadOnly();
                    return new Unsubscriber(this, observer);
                }
            }

            observer.OnCompleted();
            return new EmptyDisposable();
        }

        private void Unsubscribe(IObserver<EventEntry> observer)
        {
            lock (_syncLock)
            {
                _observers = _observers.Where(x => !observer.Equals(x)).ToList().AsReadOnly();
            }
        }

        #endregion

        #region IObserver Members

        /// <summary>
        ///     Notifies the observer that the provider has finished sending push-based notifications.
        /// </summary>
        public void OnCompleted()
        {
            ReadOnlyCollection<IObserver<EventEntry>> currentObservers = GetObserversAndLock();
            if (currentObservers != null)
            {
                Parallel.ForEach(currentObservers, observer => observer.OnCompleted());
            }
        }

        /// <summary>
        ///     Notifies the observer that the provider has experienced an error condition.
        /// </summary>
        /// <param name="error">An object that provides additional information about the error.</param>
        public void OnError(Exception error)
        {
            ReadOnlyCollection<IObserver<EventEntry>> currentObservers = GetObserversAndLock();

            if (currentObservers != null)
            {
                Parallel.ForEach(currentObservers, observer => observer.OnError(error));
            }
        }

        /// <summary>
        ///     Provides the observer with new data.
        /// </summary>
        /// <param name="value">The current notification information.</param>
        public void OnNext(EventEntry value)
        {
            foreach (var observer in _observers)
            {
                observer.OnNext(value);
            }
        }

        private ReadOnlyCollection<IObserver<EventEntry>> GetObserversAndLock()
        {
            lock (_syncLock)
            {
                if (!_isLocked)
                {
                    _isLocked = true;
                    ReadOnlyCollection<IObserver<EventEntry>> copy = _observers;
                    _observers = new List<IObserver<EventEntry>>().AsReadOnly();
                    return copy;
                }
                return null;
            }
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        ///     Releases all resources used by the current instance and unsubscribes all the observers.
        /// </summary>
        public void Dispose()
        {
            OnCompleted();
        }

        #endregion
    }
}