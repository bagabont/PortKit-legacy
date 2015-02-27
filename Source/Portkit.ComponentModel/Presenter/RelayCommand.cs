using System;
using System.Diagnostics;
using System.Windows.Input;

namespace Portkit.ComponentModel.Presenter
{
    /// <summary>
    /// Represents an <see cref="ICommand"/> implementation.
    /// </summary>
    public class RelayCommand : ICommand
    {
        #region Fields

        private readonly Action<object> _execute;
        private readonly Predicate<object> _canExecute;

        #endregion

        #region Constructors

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
                throw new ArgumentNullException("execute");
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


        #endregion

        #region ICommand Members

        /// <summary>
        /// Fires when the command state changes.
        /// </summary>
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// Fires the <see cref="CanExecuteChanged"/> event.
        /// </summary>
        protected virtual void OnCanExecuteChanged()
        {
            var handler = CanExecuteChanged;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Check if the command can execute.
        /// </summary>
        /// <param name="parameter">State parameter</param>
        /// <returns>True if can execute, otherwise false.</returns>
        [DebuggerStepThrough]
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

        #endregion

        /// <summary>
        /// Raises the <see cref="CanExecuteChanged"/> event.
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            var handler = CanExecuteChanged;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }
    }
}
