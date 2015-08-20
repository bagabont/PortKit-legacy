using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Spb.Tv.Components.Threading
{
    public sealed class ObservableTask<TResult> : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public Task<TResult> Task { get; private set; }

        public TResult Result
        {
            get
            {
                return (Task.Status == TaskStatus.RanToCompletion) ?
                    Task.Result : default(TResult);
            }
        }

        public TaskStatus Status
        {
            get
            {
                return Task.Status;
            }
        }

        public bool IsCompleted
        {
            get
            {
                return Task.IsCompleted;
            }
        }

        public bool IsSuccessful
        {
            get
            {
                return Task.Status == TaskStatus.RanToCompletion;
            }
        }

        public bool IsCanceled
        {
            get
            {
                return Task.IsCanceled;
            }
        }

        public bool IsFaulted
        {
            get
            {
                return Task.IsFaulted;
            }
        }

        public AggregateException Exception
        {
            get
            {
                return Task.Exception;
            }
        }

        public Exception InnerException
        {
            get
            {
                return (Exception == null) ? null : Exception.InnerException;
            }
        }

        public string ErrorMessage
        {
            get
            {
                return (InnerException == null) ? null : InnerException.Message;
            }
        }

        public ObservableTask(Task<TResult> task)
        {
            Task = task;

            if (!task.IsCompleted)
            {
                WatchTaskAsync(task);
            }
        }

        private void WatchTaskAsync(Task task)
        {
            try
            {
                task.ContinueWith(t =>
                {
                    var propertyChanged = PropertyChanged;
                    if (propertyChanged == null)
                    {
                        return;
                    }

                    propertyChanged(this, new PropertyChangedEventArgs("Status"));
                    propertyChanged(this, new PropertyChangedEventArgs("IsCompleted"));

                    if (task.IsCanceled)
                    {
                        propertyChanged(this, new PropertyChangedEventArgs("IsCanceled"));
                    }
                    else if (task.IsFaulted)
                    {
                        propertyChanged(this, new PropertyChangedEventArgs("IsFaulted"));
                        propertyChanged(this, new PropertyChangedEventArgs("Exception"));
                        propertyChanged(this, new PropertyChangedEventArgs("InnerException"));
                        propertyChanged(this, new PropertyChangedEventArgs("ErrorMessage"));
                    }
                    else
                    {
                        propertyChanged(this, new PropertyChangedEventArgs("IsSuccessful"));
                        propertyChanged(this, new PropertyChangedEventArgs("Result"));
                    }
                });
            }
            catch
            {
            }
        }
    }
}