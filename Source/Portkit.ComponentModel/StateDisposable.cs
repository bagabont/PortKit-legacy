using System;

namespace Portkit.ComponentModel
{
    /// <summary>
    /// An object that stores disposed state.
    /// </summary>
    public abstract class StateDisposable : IStateDisposable
    {
        /// <summary>
        /// Gets a value indicating if the object is disposed.
        /// </summary>
        public bool IsDisposed { get; private set; }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Releases resources held by the object.
        /// </summary>
        public virtual void Dispose(bool disposing)
        {
            lock (this)
            {
                if (disposing && !IsDisposed)
                {
                    IsDisposed = true;
                    GC.SuppressFinalize(this);
                }
            }
        }

        /// <summary>
        /// Releases resources before the object is reclaimed by garbage collection.
        /// </summary>
        ~StateDisposable()
        {
            Dispose(false);
        }
    }
}
