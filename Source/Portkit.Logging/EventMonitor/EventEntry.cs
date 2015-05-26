using System;
using System.Collections.ObjectModel;
using System.Diagnostics.Tracing;
using System.Globalization;
using System.Linq;

namespace Portkit.Logging.EventMonitor
{
    /// <summary>
    /// Represents an event entry class.
    /// </summary>
    public class EventEntry
    {
        internal const string DefaultDateTimeFormat = "O";

        /// <summary>
        /// Creates a new instance of the <see cref="EventEntry"/> class.
        /// </summary>
        public EventEntry(Guid sourceId, int eventId, string formattedMessage, ReadOnlyCollection<object> payload,
            DateTimeOffset timestamp)
        {
            ProviderId = sourceId;
            EventId = eventId;
            FormattedMessage = formattedMessage;
            Payload = payload;
            Timestamp = timestamp;
        }

        /// <summary>
        /// Gets the event source provider ID.
        /// </summary>
        public Guid ProviderId { get; private set; }

        /// <summary>
        /// Gets the event ID.
        /// </summary>
        public int EventId { get; private set; }

        /// <summary>
        /// Gets the event payload messages.
        /// </summary>
        public ReadOnlyCollection<object> Payload { get; private set; }

        /// <summary>
        /// Gets the time stamp when the event has occurred.
        /// </summary>
        public DateTimeOffset Timestamp { get; private set; }

        /// <summary>
        /// Gets the event's formatted message.
        /// </summary>
        public string FormattedMessage { get; private set; }

        /// <summary>
        /// Gets the event level.
        /// </summary>
        public EventLevel Level { get; private set; }

        /// <summary>
        /// Gets the event's opcode
        /// </summary>
        public EventOpcode Opcode { get; private set; }

        /// <summary>
        /// Gets the task that applies to the event.
        /// </summary>
        public EventTask Task { get; private set; }

        /// <summary>
        /// Gets the event's keywords.
        /// </summary>
        public EventKeywords Keywords { get; private set; }

        /// <summary>
        /// Factory method to create an event from <see cref="EventWrittenEventArgs"/>.
        /// </summary>
        /// <param name="args"><see cref="EventWrittenEventArgs"/> arguments.</param>
        /// <returns><see cref="EventEntry"/></returns>
        public static EventEntry Create(EventWrittenEventArgs args)
        {
            if (args == null)
            {
                throw new ArgumentNullException("args");
            }

            DateTimeOffset timestamp = DateTimeOffset.Now;

            string formattedMessage = null;
            if (args.Message != null)
            {
                formattedMessage = string.Format(CultureInfo.InvariantCulture, args.Message, args.Payload.ToArray());
            }
            var entry = new EventEntry(args.EventSource.Guid, args.EventId, formattedMessage, args.Payload, timestamp)
            {
                Level = args.Level,
                Opcode = args.Opcode,
                Task = args.Task,
                Keywords = args.Keywords
            };
            return entry;
        }

        /// <summary>
        /// Gets a formatted UTC date time string.
        /// </summary>
        /// <param name="format">Date time format.</param>
        public string GetFormattedTimestamp(string format)
        {
            return Timestamp.UtcDateTime.ToString(string.IsNullOrWhiteSpace(format) ? DefaultDateTimeFormat : format,
                CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Converts the <see cref="EventEntry"/> object to a formatted content string.
        /// </summary>
        /// <param name="formatter">Content formatter.</param>
        public string ToString(ContentFormatter formatter)
        {
            try
            {
                return formatter.Write(this);
            }
            catch (Exception)
            {
                // Ignore
            }
            return null;
        }
    }
}