using System;
using System.Diagnostics.Tracing;

namespace Portkit.Logging
{
    /// <summary>
    ///     Represents a class that can intercept log messages and diverts them from ETW to custom
    ///     <see cref="EventEntryMonitor" />.
    /// </summary>
    public sealed class ObservableEventListener : EventListener, IObservable<EventEntry>
    {
        #region Fields

        private readonly EventEntryMonitor _entryMonitor = new EventEntryMonitor();

        #endregion

        #region Methods

        /// <summary>
        ///     Notifies the provider that an observer is to receive notifications.
        /// </summary>
        /// <returns>
        ///     A reference to an interface that allows observers to stop receiving notifications before the provider has finished
        ///     sending them.
        /// </returns>
        /// <param name="observer">The object that is to receive notifications.</param>
        public IDisposable Subscribe(IObserver<EventEntry> observer)
        {
            return _entryMonitor.Subscribe(observer);
        }

        /// <summary>
        ///     Called whenever an event has been written by an event source for which the event listener has enabled events.
        /// </summary>
        /// <param name="eventData">The event arguments that describe the event.</param>
        protected override void OnEventWritten(EventWrittenEventArgs eventData)
        {
            if (eventData == null)
            {
                throw new ArgumentNullException("eventData");
            }

            EventEntry entry = EventEntry.Create(eventData);

            _entryMonitor.OnNext(entry);
        }

        #endregion

        #region IDisposable override

        /// <summary>
        ///     Releases the resources used by the current instance of the
        ///     <see cref="T:System.Diagnostics.Tracing.EventListener" /> class.
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();
            _entryMonitor.Dispose();
        }

        #endregion
    }
}