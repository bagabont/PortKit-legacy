using System;

namespace Portkit.Core
{
    /// <summary>
    /// Generic arguments class to pass to event handlers that need to receive data.
    /// </summary>
    /// <typeparam name="TData">The type of data to pass.</typeparam>
    public class DataEventArgs<TData> : EventArgs
    {
        /// <summary>
        /// Gets the information related to the event.
        /// </summary>
        public TData Value { get; private set; }

        /// <summary>
        /// Initializes the DataEventArgs class.
        /// </summary>
        /// <param name="input">Information related to the event.</param>
        public DataEventArgs(TData input)
        {
            Value = input;
        }
    }
}