using System;

namespace Portkit.ComponentModel
{
    /// <summary>
    /// An object that stores disposed state.
    /// </summary>
    public interface IStateDisposable : IDisposable
    {
        /// <summary>
        /// Gets a value indicating if the object is disposed.
        /// </summary>
        bool IsDisposed { get; }
    }
}
