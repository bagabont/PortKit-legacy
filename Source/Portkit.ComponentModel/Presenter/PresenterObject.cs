using Portkit.ComponentModel.Threading;
using System;
using System.ComponentModel;
using System.Runtime.Serialization;

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
    }
}
