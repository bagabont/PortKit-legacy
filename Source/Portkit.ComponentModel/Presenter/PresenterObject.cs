using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using Portkit.ComponentModel.Threading;
using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using Portkit.Core.Extensions;

namespace Portkit.ComponentModel.Presenter
{
    /// <summary>
    /// Represents a base object that implements <see cref="INotifyPropertyChanged"/> and raises it on UI thread.
    /// </summary>
    [DataContract]
    public abstract class PresenterObject : ObservableObject
    {
        /// <summary>
        /// Gets or sets the dispatcher used for thread synchronization in all instances of the class.
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public static IThreadDispatcher UIDispatcher { get; set; }

        /// <summary>
        /// Raises the PropertyChanged event on UI thread.
        /// </summary>
        /// <param name="propertyName">The name of the property that is changed.</param>
        protected override void OnPropertyChanged(string propertyName)
        {
            if (UIDispatcher == null)
            {
                throw new InvalidOperationException("Cannot raise event on UI thread. UIDispatcher is null.");
            }

            if (!UIDispatcher.HasThreadAccess)
            {
                UIDispatcher.Run(() =>
                    base.OnPropertyChanged(propertyName));
            }
            else
            {
                base.OnPropertyChanged(propertyName);
            }
        }

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
