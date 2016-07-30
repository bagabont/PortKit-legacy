using System;
using Windows.UI.Core;
using Windows.UI.Xaml;

namespace Portkit.ComponentModel
{
    /// <summary>
    /// Represents an adapter that can be passed to <see cref="IThreadDispatcher"/>.
    /// </summary>
    public class CoreDispatcherAdapter : IThreadDispatcher
    {
        private static CoreDispatcher _dispatcher;

        public bool HasThreadAccess => _dispatcher.HasThreadAccess;

        /// <summary>
        /// Attaches to the current window dispatcher.
        /// </summary>
        public void Attach()
        {
            _dispatcher = Window.Current.Dispatcher;
        }

        /// <summary>
        /// Attaches to the specified dispatcher.
        /// </summary>
        /// <param name="dispatcher"><see cref="CoreDispatcher"/> instance used for dispatching.</param>
        public void Attach(CoreDispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

        public async void Run(Action action)
        {
            if (_dispatcher == null)
            {
                throw new InvalidOperationException("Dispatcher not found. Please, attach to dispatcher before running an action.");
            }
            // If the core dispatcher has access, simply execute the action
            // otherwise dispatch it.
            if (_dispatcher.HasThreadAccess)
            {
                action();
            }
            else
            {
                await _dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => action());
            }
        }
    }
}