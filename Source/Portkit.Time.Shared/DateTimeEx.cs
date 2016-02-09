using System;

namespace Portkit.Time
{
    /// <summary>
    /// Data time extensions class.
    /// </summary>
    public static class DateTimeEx
    {
        /// <summary>
        /// Convert a date time vale to UTC UNIX format timestamp.
        /// </summary>
        /// <param name="date">Source date.</param>
        /// <returns>Int32 representation of a UTC UNIX timestamp</returns>
        public static int ToUnixUtcTimeStamp(this DateTime date)
        {
            int unixTimeStamp;
            var zuluTime = date.ToUniversalTime();
            var unixEpoch = new DateTime(1970, 1, 1);
            unixTimeStamp = (int)(zuluTime.Subtract(unixEpoch)).TotalSeconds;
            return unixTimeStamp;
        }
    }
}
