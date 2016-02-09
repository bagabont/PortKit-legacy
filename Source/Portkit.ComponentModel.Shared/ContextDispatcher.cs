using System;
using System.Threading;

namespace Portkit.ComponentModel
{
    /// <inheritdoc/>
    public class ContextDispatcher : IThreadDispatcher
    {
        private static SynchronizationContext _context;
        private static volatile ContextDispatcher _default;
        private static readonly object _syncRoot = new object();

        /// <summary>
        /// Gets the default instance of <see cref="ContextDispatcher"/>
        /// </summary>
        public static ContextDispatcher Default
        {
            get
            {
                if (_default == null)
                {
                    lock (_syncRoot)
                    {
                        if (_default == null)
                        {
                            _default = new ContextDispatcher();
                        }
                    }
                }
                return _default;
            }
        }

        /// <inheritdoc />
        public void Attach()
        {
            Attach(SynchronizationContext.Current);
        }

        /// <summary>
        /// Attaches the instance of the dispatcher to a thread.
        /// </summary>
        /// <param name="context">SynchronizationContext to attach to.</param>
        /// <exception cref="ArgumentNullException">When synchronization context is null.</exception>
        public void Attach(SynchronizationContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context), "Unable to find a suitable SynchronizationContext instance.");
            }
            _context = context;
        }

        /// <inheritdoc />
        public bool HasThreadAccess => SynchronizationContext.Current == _context;

        /// <inheritdoc />
        public void Run(Action actionDelegate)
        {
            try
            {
                _context.Post(callback => actionDelegate(), null);
            }
            catch (Exception e)
            {
                const string msg = "Unable to invoke action delegate. Most likely the dispatcher is not attached to the correct SynchronizationContext. Check inner exception for more details.";
                throw new InvalidOperationException(msg, e);
            }
        }
    }
}
