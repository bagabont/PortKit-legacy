using System.Globalization;
using System.IO;

namespace Portkit.Logging
{
    /// <summary>
    /// Base class to format entry content as string.
    /// </summary>
    public abstract class ContentFormatter
    {
        /// <summary>
        /// Writes an entry to a <see cref="TextWriter"/>
        /// </summary>
        /// <param name="eventEntry">Source event entry.</param>
        /// <param name="writer">Text writer to write the event to.</param>
        public abstract void Write(EventEntry eventEntry, TextWriter writer);

        /// <summary>
        /// Writes an entry to a <see cref="TextWriter"/>
        /// </summary>
        /// <param name="eventEntry">Source event entry.</param>
        public string Write(EventEntry eventEntry)
        {
            using (var writer = new StringWriter(CultureInfo.CurrentCulture))
            {
                Write(eventEntry, writer);
                return writer.ToString();
            }
        }
    }
}