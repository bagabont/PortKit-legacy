using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Portkit.ComponentModel
{
    /// <summary>
    /// Provides basic decoupled messaging.
    /// </summary>
    public sealed class MessageBus : IMessageBus
    {
        private readonly Dictionary<Type, List<Object>> _subscribers;
        private static readonly object SyncRoot = new Object();
        private static volatile MessageBus _default;

        /// <summary>
        /// Gets an instance of <see cref="MessageBus"/> class.
        /// </summary>
        public static IMessageBus Default
        {
            get
            {
                if (_default == null)
                {
                    lock (SyncRoot)
                    {
                        if (_default == null)
                        {
                            _default = new MessageBus();
                        }
                    }
                }
                return _default;
            }
        }

        private MessageBus()
        {
            _subscribers = new Dictionary<Type, List<Object>>();
        }

        /// <inheritdoc/>
        public void Subscribe<TMessage>(Action<TMessage> handler)
        {
            if (_subscribers.ContainsKey(typeof(TMessage)))
            {
                var handlers = _subscribers[typeof(TMessage)];
                handlers.Add(handler);
            }
            else
            {
                var handlers = new List<Object>
                {
                    handler
                };
                _subscribers[typeof(TMessage)] = handlers;
            }
        }

        /// <inheritdoc/>
        public void Unsubscribe<TMessage>(Action<TMessage> handler)
        {
            if (!_subscribers.ContainsKey(typeof(TMessage)))
            {
                return;
            }
            var handlers = _subscribers[typeof(TMessage)];
            handlers.Remove(handler);

            if (handlers.Count == 0)
            {
                _subscribers.Remove(typeof(TMessage));
            }
        }

        /// <inheritdoc/>
        public void Publish<TRecipient, TMessage>(TMessage message)
        {
            var handlers = GetActions(typeof(TMessage));
            if (handlers == null)
            {
                return;
            }
#if UNIVERSAL
            var validHandlers = handlers.Cast<Action<TMessage>>()
                .Where(h => h.Method != null && h.Method.DeclaringType == typeof(TRecipient));    
#else
            var validHandlers = handlers.Cast<Action<TMessage>>()
                .Where(h => h.GetMethodInfo() != null && h.GetMethodInfo().DeclaringType == typeof(TRecipient));
#endif
            foreach (var handler in validHandlers)
            {
                handler.Invoke(message);
            }
        }

        /// <inheritdoc/>
        public void Publish<TMessage>(TMessage message)
        {
            var handlers = GetActions(typeof(TMessage));
            if (handlers == null)
            {
                return;
            }
            foreach (Action<TMessage> handler in handlers)
            {
                handler.Invoke(message);
            }
        }

        /// <inheritdoc/>
        public void Publish(Object message)
        {
            var messageType = message.GetType();
            if (!_subscribers.ContainsKey(messageType))
            {
                return;
            }
            var handlers = _subscribers[messageType];
            foreach (var handler in handlers)
            {
                var actionType = handler.GetType();
#if UNIVERSAL
                var invoke = actionType.GetMethod("Invoke", new[] { messageType });
#else
                var invoke = actionType.GetRuntimeMethod("Invoke", new[] { messageType });
#endif
                invoke.Invoke(handler, new[] { message });
            }
        }

        /// <summary>
        /// Sets the default (static) instance of the MessaageBus to null.
        /// </summary>
        public void Reset()
        {
            _default = null;
        }

        private IEnumerable<object> GetActions(Type messageType)
        {
            List<object> value;
            _subscribers.TryGetValue(messageType, out  value);
            return value;
        }
    }
}
