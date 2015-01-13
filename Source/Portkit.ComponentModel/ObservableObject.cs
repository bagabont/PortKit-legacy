using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using Portkit.Core.Extensions;

namespace Portkit.ComponentModel
{
    /// <summary>
    /// Provides an abstract implementation of the INotifyPropertyChanged interface.
    /// </summary>
    [DataContract]
    public abstract class ObservableObject : INotifyPropertyChanged
    {
        /// <summary>
        /// Raised when a property of the observable object is changed.
        /// </summary>
        public virtual event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Fires the PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">The name of the property that changed.</param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

#if UNIVERSAL

        /// <summary>
        /// Sets a property value and fires the PropertyChanged event if the value is changed.
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
#endif

        /// <summary>
        /// Fires the PropertyChanged event.
        /// </summary>
        /// <typeparam name="T">Property type.</typeparam>
        /// <param name="propertyExpression">Property expression, that provides the name of the property that changed,
        ///  in order to avoid using "magic strings".</param>
        protected virtual void SetProperty<T>(Expression<Func<T>> propertyExpression)
        {
            var propertyName = propertyExpression.GetPropertyName();
            OnPropertyChanged(propertyName);
        }
    }
}
