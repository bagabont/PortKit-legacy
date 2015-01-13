using System;

namespace Portkit.Core.Extensions
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
        public static Int32 UnixTimeStampUtc(this System.DateTime date)
        {
            Int32 unixTimeStamp;
            var zuluTime = date.ToUniversalTime();
            var unixEpoch = new System.DateTime(1970, 1, 1);
            unixTimeStamp = (Int32)(zuluTime.Subtract(unixEpoch)).TotalSeconds;
            return unixTimeStamp;
        }
    }
}
