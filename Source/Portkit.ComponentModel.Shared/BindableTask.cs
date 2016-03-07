using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Portkit.ComponentModel
{
    /// <summary>
    /// Represents an observable task with properties for getting results and error details. 
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    public sealed class BindableTask<TResult> : INotifyPropertyChanged
    {
        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        private readonly TResult _defaultResult;

        /// <summary>
        /// Gets the <see cref="Task"/> that is being observed.
        /// </summary>
        public Task<TResult> Task { get; }

        /// <summary>
        /// Gets the result value of this <see cref="Task"/>.
        /// </summary>
        /// 
        /// <returns>
        /// The result value of this <see cref="Task"/>, which is the same type as the task's type parameter.
        /// </returns>
        public TResult Result => IsSuccessful ? Task.Result : _defaultResult;

        /// <summary>
        /// Gets the <see cref="Task"/> of this task.
        /// </summary>
        /// 
        /// <returns>
        /// The current <see cref="TaskStatus"/> of this task instance.
        /// </returns>
        public TaskStatus Status => Task.Status;

        /// <summary>
        /// The task completed execution.
        /// </summary>
        public bool IsCompleted => Task.IsCompleted;

        /// <summary>
        /// The task completed execution successful.
        /// </summary>
        public bool IsSuccessful => Task.Status == TaskStatus.RanToCompletion;

        /// <summary>
        /// Gets whether this <see cref="Task"/> instance has completed execution due to being canceled.
        /// </summary>
        /// 
        /// <returns>
        /// true if the task has completed due to being canceled; otherwise false.
        /// </returns>
        public bool IsCanceled => Task.IsCanceled;

        /// <summary>
        /// Gets whether the <see cref="Task"/> completed due to an unhandled exception.
        /// </summary>
        /// 
        /// <returns>
        /// true if the task has thrown an unhandled exception; otherwise false.
        /// </returns>
        public bool IsFaulted => Task.IsFaulted;

        /// <summary>
        /// Gets the <see cref="AggregateException"/> that caused the <see cref="Task"/> to end prematurely. 
        /// If the <see cref="Task"/> completed successfully or has not yet thrown any exceptions, this will return null.
        /// </summary>
        /// 
        /// <returns>
        /// The <see cref="AggregateException"/> that caused the <see cref="Task"/> to end prematurely.
        /// </returns>
        public AggregateException Exception => Task.Exception;

        /// <summary>
        /// Gets the <see cref="Exception"/> instance, that caused the current exception.
        /// </summary>
        public Exception InnerException => Exception?.InnerException;

        /// <summary>
        /// Gets a message that describes the current exception.
        /// </summary>
        public string ErrorMessage => InnerException?.Message;

        /// <summary>
        /// Creates a new instance of the <see cref="Attach"/> class.
        /// </summary>
        /// <param name="task">Running task that is observed.</param>
        /// <param name="defaultResult">Default result to initialize the task with.</param>
        /// <param name="continueOnCapturedContext">true to attempt to marshal the continuation back to the original context captured; otherwise, false.</param>
        public BindableTask(Task<TResult> task, TResult defaultResult, bool continueOnCapturedContext = true)
        {
            _defaultResult = defaultResult;
            Task = System.Threading.Tasks.Task.Run(() => task);

            if (!task.IsCompleted)
            {
                Attach(task, continueOnCapturedContext);
            }
        }

        /// <summary>
        /// Creates a new instance of the <see cref="Attach"/> class.
        /// </summary>
        /// <param name="task">Running task that is observed.</param>
        /// <param name="continueOnCapturedContext">true to attempt to marshal the continuation back to the original context captured; otherwise, false.</param>
        public BindableTask(Task<TResult> task, bool continueOnCapturedContext = true) :
            this(task, default(TResult), continueOnCapturedContext)
        {
        }

        /// <summary>
        /// This method takes a task representing the asynchronous operation, and (asynchronously) waits for it to complete.
        /// </summary>
        /// <param name="task">Asynchronous operation.</param>
        /// <param name="continueOnCapturedContext">true to attempt to marshal the continuation back to the original context captured; otherwise, false.</param>
        private void Attach(Task task, bool continueOnCapturedContext)
        {
            if (continueOnCapturedContext)
            {
                task.ContinueWith(NotifyObserver, TaskScheduler.FromCurrentSynchronizationContext());
            }
            else
            {
                task.ContinueWith(NotifyObserver);
            }
        }

        private void NotifyObserver(Task task)
        {
            var handler = PropertyChanged;
            if (handler == null)
            {
                return;
            }

            handler(this, new PropertyChangedEventArgs(nameof(Status)));
            handler(this, new PropertyChangedEventArgs(nameof(IsCompleted)));

            if (task.IsCanceled)
            {
                handler(this, new PropertyChangedEventArgs(nameof(IsCanceled)));
            }
            else if (task.IsFaulted)
            {
                handler(this, new PropertyChangedEventArgs(nameof(IsFaulted)));
                handler(this, new PropertyChangedEventArgs(nameof(Exception)));
                handler(this, new PropertyChangedEventArgs(nameof(InnerException)));
                handler(this, new PropertyChangedEventArgs(nameof(ErrorMessage)));
            }
            else
            {
                handler(this, new PropertyChangedEventArgs(nameof(IsSuccessful)));
                handler(this, new PropertyChangedEventArgs(nameof(Result)));
            }
        }
    }
}