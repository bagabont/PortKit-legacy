using System;
using System.Diagnostics;

namespace Portkit.ComponentModel.Presenter
{
    /// <summary>
    /// Represents a relay command class of a generic type for delegating methods.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RelayCommand<T> : RelayCommand
    {
        #region Constructors

        /// <summary>
        /// Creates a new instance of the class.
        /// </summary>
        /// <param name="execute">Method to be executed when the command is called.</param>
        public RelayCommand(Action<T> execute)
            : base(o => execute((T)o), o => true)
        {

        }

        /// <summary>
        /// Creates a new instance of the class.
        /// </summary>
        /// <param name="execute">Method to be executed when the command is called.</param>
        /// <param name="canExecute">Predicate to check if the command can execute.</param>
        public RelayCommand(Action<T> execute, Predicate<T> canExecute)
            : base(o => execute((T)o), o => canExecute((T)o))
        {

        }

        /// <summary>
        /// Creates a new instance of the class.
        /// </summary>
        /// <param name="execute">Method to be executed when the command is called.</param>
        /// <param name="canExecute">Predicate to check if the command can execute.</param>
        public RelayCommand(Action<T> execute, Func<bool> canExecute)
            : base(o => execute((T)o), o => canExecute())
        {

        }

        #endregion

        #region ICommand Members

        /// <summary>
        /// Checks if the command can be executed.
        /// </summary>
        /// <param name="parameter">Parameter to pass.</param>
        /// <returns>True if the command can be executed, otherwise false.</returns>
        [DebuggerStepThrough]
        public bool CanExecute(T parameter)
        {
            return base.CanExecute(parameter);
        }

        /// <summary>
        /// Executes the delegate method.
        /// </summary>
        /// <param name="parameter">Parameter to be passed to the delegate method.</param>
        public void Execute(T parameter)
        {
            base.Execute(parameter);
        }

        #endregion
    }
}
