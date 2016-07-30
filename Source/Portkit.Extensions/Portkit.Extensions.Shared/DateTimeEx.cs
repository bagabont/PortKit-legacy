using System;

namespace Portkit.Extensions
{
    /// <summary>
    /// <see cref="DateTime"/> extensions class.
    /// </summary>
    public static class DateTimeEx
    {
        /// <summary>
        /// Convert a date time vale to UTC UNIX format timestamp.
        /// </summary>
        /// <param name="dateTime"><see cref="DateTime"/> to convert.</param>
        /// <returns><see cref="int"/> representation of a UTC UNIX timestamp</returns>
        public static int ToUnixUtcTimeStamp(this DateTime dateTime)
        {
            var zuluTime = dateTime.ToUniversalTime();
            var unixEpoch = new DateTime(1970, 1, 1);
            var unixTimeStamp = (int)zuluTime.Subtract(unixEpoch).TotalSeconds;
            return unixTimeStamp;
        }

        /// <summary>
        /// Converts a UNIX timestamp (in seconds) to its UTC <see cref="DateTime"/> equivalent.
        /// </summary>
        /// <param name="timeStamp"><see cref="int"/> UNIX timestamp in seconds.</param>
        /// <returns>UTC <see cref="DateTime"/> equivalent of the UNIX timestamp.</returns>
        public static DateTime TimeStampToDateTime(this double timeStamp)
        {
            var date = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            return date.AddSeconds(timeStamp);
        }

        /// <summary>
        /// Round up a <see cref="DateTime"/> to a <see cref="TimeSpan"/>.
        /// </summary>
        /// <param name="dateTime"><see cref="DateTime"/> date to round</param>
        /// <param name="delta"><see cref="TimeSpan"/> time to round up to.</param>
        /// <returns>Rounded <see cref="DateTime"/></returns>
        public static DateTime RoundUp(this DateTime dateTime, TimeSpan delta)
        {
            var deltaTicks = (delta.Ticks - (dateTime.Ticks % delta.Ticks)) % delta.Ticks;
            return new DateTime(dateTime.Ticks + deltaTicks, dateTime.Kind);
        }

        /// <summary>
        /// Round down a <see cref="DateTime"/> to a <see cref="TimeSpan"/>.
        /// </summary>
        /// <param name="dateTime"><see cref="DateTime"/> date to round</param>
        /// <param name="delta"><see cref="TimeSpan"/> time to round down to.</param>
        /// <returns>Rounded <see cref="DateTime"/></returns>
        public static DateTime RoundDown(this DateTime dateTime, TimeSpan delta)
        {
            var deltaTicks = dateTime.Ticks % delta.Ticks;
            return new DateTime(dateTime.Ticks - deltaTicks, dateTime.Kind);
        }

        /// <summary>
        /// Rounds a <see cref="DateTime"/> to the nearest <see cref="TimeSpan"/>.
        /// </summary>
        /// <param name="dateTime"><see cref="DateTime"/> date to round</param>
        /// <param name="delta"><see cref="TimeSpan"/> time to round to.</param>
        /// <returns>Rounded <see cref="DateTime"/></returns>
        public static DateTime RoundToNearest(this DateTime dateTime, TimeSpan delta)
        {
            var deltaTicks = dateTime.Ticks % delta.Ticks;
            var roundUp = deltaTicks > delta.Ticks / 2;
            return roundUp ? dateTime.RoundUp(delta) : dateTime.RoundDown(delta);
        }
    }
}
