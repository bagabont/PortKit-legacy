using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace Portkit.ComponentModel
{
    /// <summary>
    /// Provides an abstract implementation of the INotifyPropertyChanged interface that supports 
    /// dispatching the <see cref="PropertyChanged"/> event to another thread. 
    /// </summary>
    public abstract class BindableObject : INotifyPropertyChanged
    {
        private static IThreadDispatcher _dispatcher;

        /// <summary>
        /// Enables or disables thread dispatching on <see cref="PropertyChanged"/>.
        /// </summary>
        protected virtual bool DispatchPropertyChanges { get; set; } = true;

        /// <summary>
        /// Raised when a property of the object is changed.
        /// </summary>
        public virtual event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Registers a UI Dispatcher which
        /// </summary>
        /// <param name="dispatcher"></param>
        public static void RegisterDispatcher(IThreadDispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

        /// <summary>
        /// Raises the <see cref="PropertyChanged"/> event.
        /// </summary>
        /// <param name="propertyName">The name of the property that changed.</param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler == null)
            {
                return;
            }
            var args = new PropertyChangedEventArgs(propertyName);
            var dispatch = DispatchPropertyChanges && _dispatcher != null && !_dispatcher.HasThreadAccess;
            if (dispatch)
            {
                _dispatcher.Run(() => handler.Invoke(this, args));
            }
            else
            {
                handler(this, args);
            }
        }

        /// <summary>
        /// Sets a property value and fires the <see cref="PropertyChanged"/> event if the value is changed.
        /// </summary>
        /// <typeparam name="T">Property type.</typeparam>
        /// <param name="storage">Backing field reference.</param>
        /// <param name="value">New property value.</param>
        /// <param name="propertyName">Property name.</param>
        /// <returns>True if the property value is changed, false if the value instance is considered to be equal.</returns>
        protected virtual bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value))
            {
                return false;
            }
            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        /// <summary>
        /// Fires the PropertyChanged event.
        /// </summary>
        /// <typeparam name="T">Property type.</typeparam>
        /// <param name="propertyExpression">Property expression, that provides the name of the property that changed,
        ///  in order to avoid using "magic strings".</param>
        protected virtual void SetProperty<T>(Expression<Func<T>> propertyExpression) =>
            OnPropertyChanged(PropertySupport.ExtractPropertyName(propertyExpression));
    }
}