using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace Portkit.Logging.EventMonitor
{
    /// <summary>
    /// Formats entry content as text string.
    /// </summary>
    public sealed class TextContentFormatter : ContentFormatter
    {
        internal const string ProviderId = "ProviderId";
        internal const string EventId = "EventId";
        internal const string Keywords = "Keywords";
        internal const string Level = "Level";
        internal const string Message = "Message";
        internal const string Opcode = "Opcode";
        internal const string Task = "Task";
        internal const string Version = "Version";
        internal const string Payload = "Payload";
        internal const string EventName = "EventName";
        internal const string Timestamp = "Timestamp";

        private string _dateTimeFormat;

        /// <summary>
        /// Gets or sets the format of the timestamp.
        /// </summary>
        public string DateTimeFormat
        {
            get { return _dateTimeFormat; }

            set
            {
                ValidDateTimeFormat(value, "DateTimeFormat");
                _dateTimeFormat = value;
            }
        }

        /// <summary>
        /// Writes an entry to a <see cref="TextWriter"/>
        /// </summary>
        /// <param name="eventEntry">Source event entry.</param>
        /// <param name="writer">Text writer to write the event to.</param>
        public override void Write(EventEntry eventEntry, TextWriter writer)
        {
            if (eventEntry == null)
            {
                throw new ArgumentNullException("eventEntry");
            }
            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }

            const string format = "{0} : {1}";

            writer.WriteLine(format, ProviderId, eventEntry.ProviderId);

            writer.WriteLine(format, EventId, eventEntry.EventId);

            writer.WriteLine(format, Level, eventEntry.Level);

            writer.WriteLine(format, Message, eventEntry.FormattedMessage);

            writer.WriteLine(format, Keywords, eventEntry.Keywords);

            writer.WriteLine(format, Opcode, eventEntry.Opcode);

            writer.WriteLine(format, Payload, FormatPayload(eventEntry));

            writer.WriteLine(format, Timestamp, eventEntry.GetFormattedTimestamp(DateTimeFormat));

            writer.WriteLine();
        }

        private static void ValidDateTimeFormat(string format, string argumentName)
        {
            if (format == null)
            {
                return;
            }

            try
            {
                DateTime.Now.ToString(format, CultureInfo.InvariantCulture);
            }
            catch (FormatException e)
            {
                throw new ArgumentException("The date time format is invalid.", e);
            }
        }

        private static string FormatPayload(EventEntry entry)
        {
            var sb = new StringBuilder();

            for (int i = 0; i < entry.Payload.Count; i++)
            {
                try
                {
                    sb.AppendFormat("[{0} : {1}] ", i, entry.Payload[i]);
                }
                catch (Exception e)
                {
                    sb.AppendFormat("[{0} : {1}] ", "Exception",
                        string.Format("Cannot serialize payload: {0}", e.Message));
                }
            }

            return sb.ToString();
        }
    }
}