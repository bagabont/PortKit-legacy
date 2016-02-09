using System;
using System.IO;

namespace Portkit.Utils
{
    /// <summary>
    /// Stream extensions class.
    /// </summary>
    public static class StreamUtils
    {
        // System.IO.Stream
        /// <summary>Reads the bytes from the current stream and writes them to the destination stream.</summary>
        /// <param name="source">Source stream</param>
        /// <param name="destination">The stream that will contain the contents of the current stream.</param>
        /// <exception cref="T:System.ArgumentNullException">
        ///   <paramref name="destination" /> is null.</exception>
        /// <exception cref="T:System.NotSupportedException">The current stream does not support reading.-or-<paramref name="destination" /> does not support writing.</exception>
        /// <exception cref="T:System.ObjectDisposedException">Either the current stream or <paramref name="destination" /> were closed before the <see cref="M:System.IO.Stream.CopyTo(System.IO.Stream)" /> method was called.</exception>
        /// <exception cref="T:System.IO.IOException">An I/O error occurred.</exception>
        public static void CopyTo(Stream source, Stream destination)
        {
            if (destination == null)
            {
                throw new ArgumentNullException(nameof(destination));
            }
            if (!source.CanRead && !source.CanWrite)
            {
                throw new ObjectDisposedException(null, "Stream closed");
            }
            if (!destination.CanRead && !destination.CanWrite)
            {
                throw new ObjectDisposedException("destination", "Stream closed");
            }
            if (!source.CanRead)
            {
                throw new NotSupportedException("Unreadable Stream");
            }
            if (!destination.CanWrite)
            {
                throw new NotSupportedException("Unwritable Stream");
            }
            InternalCopyTo(source, destination, 81920);
        }

        /// <summary> Attempts to seek to beginning of the stream. </summary>
        /// <param name="source">Stream source.</param>
        /// <returns>True if the position has been successfully set to the beginning of the stream, otherwise false.</returns>
        public static bool TryResetPosition(Stream source)
        {
            if (source == null || !source.CanSeek)
            {
                return false;
            }
            source.Seek(0, SeekOrigin.Begin);
            return true;
        }

        private static void InternalCopyTo(Stream source, Stream destination, int bufferSize)
        {
            var array = new byte[bufferSize];
            int count;
            while ((count = source.Read(array, 0, array.Length)) != 0)
            {
                destination.Write(array, 0, count);
            }
        }
    }
}
