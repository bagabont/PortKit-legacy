using System;

namespace Portkit.ComponentModel
{
    /// <summary>
    /// Thread dispatcher.
    /// </summary>
    public interface IThreadDispatcher
    {
        /// <summary>
        /// Gets a value that specifies whether the event dispatcher provided by this
        /// instance has access to the current thread or not.
        /// </summary>
        /// <returns>True if the event dispatcher has thread access; false it does not.</returns>
        bool HasThreadAccess { get; }

        /// <summary>
        /// When implemented dispatches an action to a thread.
        /// </summary>
        /// <param name="action">Action to be dispatched.</param>
        void Run(Action action);

        /// <summary>
        /// Attaches the instance of the dispatcher to a thread.
        /// </summary>
        void Attach();
    }
}