using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Portkit.ComponentModel.Threading
{
    /// <summary>
    /// Represents an observable task with properties for getting results and error details. 
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    public sealed class ObservableTask<TResult> : INotifyPropertyChanged
    {
        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets the <see cref="Task"/> that is being observed.
        /// </summary>
        public Task<TResult> Task { get; private set; }

        /// <summary>
        /// Gets the result value of this <see cref="Task"/>.
        /// </summary>
        /// 
        /// <returns>
        /// The result value of this <see cref="Task"/>, which is the same type as the task's type parameter.
        /// </returns>
        public TResult Result
        {
            get
            {
                return IsSuccessful ? Task.Result : default(TResult);
            }
        }

        /// <summary>
        /// Gets the <see cref="Task"/> of this task.
        /// </summary>
        /// 
        /// <returns>
        /// The current <see cref="TaskStatus"/> of this task instance.
        /// </returns>
        public TaskStatus Status
        {
            get
            {
                return Task.Status;
            }
        }

        /// <summary>
        /// The task completed execution.
        /// </summary>
        public bool IsCompleted
        {
            get
            {
                return Task.IsCompleted;
            }
        }

        /// <summary>
        /// The task completed execution successful.
        /// </summary>
        public bool IsSuccessful
        {
            get
            {
                return Task.Status == TaskStatus.RanToCompletion;
            }
        }

        /// <summary>
        /// Gets whether this <see cref="Task"/> instance has completed execution due to being canceled.
        /// </summary>
        /// 
        /// <returns>
        /// true if the task has completed due to being canceled; otherwise false.
        /// </returns>
        public bool IsCanceled
        {
            get
            {
                return Task.IsCanceled;
            }
        }

        /// <summary>
        /// Gets whether the <see cref="Task"/> completed due to an unhandled exception.
        /// </summary>
        /// 
        /// <returns>
        /// true if the task has thrown an unhandled exception; otherwise false.
        /// </returns>
        public bool IsFaulted
        {
            get
            {
                return Task.IsFaulted;
            }
        }

        /// <summary>
        /// Gets the <see cref="AggregateException"/> that caused the <see cref="Task"/> to end prematurely. 
        /// If the <see cref="Task"/> completed successfully or has not yet thrown any exceptions, this will return null.
        /// </summary>
        /// 
        /// <returns>
        /// The <see cref="AggregateException"/> that caused the <see cref="Task"/> to end prematurely.
        /// </returns>
        public AggregateException Exception
        {
            get
            {
                return Task.Exception;
            }
        }

        /// <summary>
        /// Gets the <see cref="Exception"/> instance, that caused the current exception.
        /// </summary>
        public Exception InnerException
        {
            get
            {
                return (Exception == null) ? null : Exception.InnerException;
            }
        }

        /// <summary>
        /// Gets a message that describes the current exception.
        /// </summary>
        public string ErrorMessage
        {
            get
            {
                return (InnerException == null) ? null : InnerException.Message;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="task"></param>
        /// <param name="continueOnCapturedContext">true to attempt to marshal the continuation back to the original context captured; otherwise, false.</param>
        public ObservableTask(Task<TResult> task, bool continueOnCapturedContext = true)
        {
            Task = task;

            if (!task.IsCompleted)
            {
                ObserveTask(task, continueOnCapturedContext);
            }
        }

        /// <summary>
        /// This method takes a task representing the asynchronous operation, and (asynchronously) waits for it to complete.
        /// </summary>
        /// <param name="task">Asynchronous operation.</param>
        /// <param name="continueOnCapturedContext">true to attempt to marshal the continuation back to the original context captured; otherwise, false.</param>
        private void ObserveTask(Task task, bool continueOnCapturedContext)
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

            handler(this, new PropertyChangedEventArgs("Status"));
            handler(this, new PropertyChangedEventArgs("IsCompleted"));

            if (task.IsCanceled)
            {
                handler(this, new PropertyChangedEventArgs("IsCanceled"));
            }
            else if (task.IsFaulted)
            {
                handler(this, new PropertyChangedEventArgs("IsFaulted"));
                handler(this, new PropertyChangedEventArgs("Exception"));
                handler(this, new PropertyChangedEventArgs("InnerException"));
                handler(this, new PropertyChangedEventArgs("ErrorMessage"));
            }
            else
            {
                handler(this, new PropertyChangedEventArgs("IsSuccessful"));
                handler(this, new PropertyChangedEventArgs("Result"));
            }
        }
    }
}