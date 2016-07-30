using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Windows.Input;

namespace Portkit.ComponentModel
{
    /// <summary>
    /// Represents an <see cref="ICommand"/> implementation.
    /// </summary>
    public class RelayCommand : ICommand
    {
        /// <summary>
        /// Fires when the command state changes.
        /// </summary>
        public event EventHandler CanExecuteChanged;
        /// <summary>
        /// Raises the <see cref="CanExecuteChanged"/> event.
        /// </summary>
        public virtual void RaiseCanExecuteChanged()
        {
            OnCanExecuteChanged();
        }

        private INotifyPropertyChanged _monitor;
        private readonly HashSet<string> _watchList = new HashSet<string>();
        private readonly Action<object> _execute;
        private readonly Predicate<object> _canExecute;

        /// <summary>
        /// Creates a new instance of the <see cref="RelayCommand"/> class.
        /// </summary>
        /// <param name="execute">Method to execute.</param>
        public RelayCommand(Action<object> execute)
            : this(execute, null)
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="RelayCommand"/> class.
        /// </summary>
        /// <param name="execute">Method to execute.</param>
        public RelayCommand(Action execute)
            : this(o => execute(), null)
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="RelayCommand"/> class.
        /// </summary>
        /// <param name="execute">Method to execute.</param>
        /// <param name="canExecute">Predicate to check if the command can execute.</param>
        public RelayCommand(Action<object> execute, Predicate<object> canExecute)
        {
            if (execute == null)
            {
                throw new ArgumentNullException(nameof(execute));
            }
            _execute = execute;
            _canExecute = canExecute;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="RelayCommand"/> class.
        /// </summary>
        /// <param name="execute">Method to execute.</param>
        /// <param name="canExecute">Predicate for the command execution policy.</param>
        public RelayCommand(Action execute, Predicate<object> canExecute)
            : this(o => execute(), canExecute)
        {

        }

        /// <summary>
        /// Creates a new instance of the <see cref="RelayCommand"/> class.
        /// </summary>
        /// <param name="execute">Method to execute.</param>
        /// <param name="canExecute">Function for the command execution policy.</param>
        public RelayCommand(Action execute, Func<bool> canExecute)
            : this(o => execute(), s => canExecute())
        {
        }

        /// <summary>
        /// Fires the <see cref="CanExecuteChanged"/> event.
        /// </summary>
        protected virtual void OnCanExecuteChanged()
        {
            var handler = CanExecuteChanged;
            handler?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Check if the command can execute.
        /// </summary>
        /// <param name="parameter">State parameter</param>
        /// <returns>True if can execute, otherwise false.</returns>
        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute(parameter);
        }

        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <param name="parameter">Parameter to pass to the delegate method.</param>
        public void Execute(object parameter)
        {
            if (CanExecute(parameter))
            {
                _execute(parameter);
            }
        }
        public RelayCommand WatchProperty<TProperty>(Expression<Func<TProperty>> propertyExpression)
        {
            AddWatchProperty(PropertySupport.ExtractPropertyName(propertyExpression));
            WireMonitor(propertyExpression.Body as MemberExpression);
            return this;
        }

        protected void AddWatchProperty(string property)
        {
            if (_watchList.Contains(property))
            {
                throw new ArgumentException($"{property} is already in the watch list.");
            }
            _watchList.Add(property);
        }

        protected void WireMonitor(MemberExpression expression)
        {
            if (expression == null || _monitor != null)
            {
                return;
            }

            _monitor = (expression.Expression as ConstantExpression)?.Value as INotifyPropertyChanged;
            if (_monitor != null)
            {
                _monitor.PropertyChanged += OnWatchedPropertyChanged;
            }
        }

        private void OnWatchedPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (_watchList.Contains(e.PropertyName))
            {
                RaiseCanExecuteChanged();
            }
        }
    }
}
