using System;

namespace Portkit.ComponentModel.Communication
{
    /// <summary>
    /// Provides basic decoupled messaging.
    /// </summary>
    public interface IMessageBus
    {
        /// <summary>
        /// Adds the message handler to the subscriptions list.
        /// </summary>
        /// <typeparam name="TMessage">Type of the message.</typeparam>
        /// <param name="handler">Handler to be executed when a message of the subscription type is published.</param>
        void Subscribe<TMessage>(Action<TMessage> handler);

        /// <summary>
        /// Removes the message handler from the subscriptions list.
        /// </summary>
        /// <typeparam name="TMessage">Type of the message.</typeparam>
        /// <param name="handler">Handler to be executed when a message is published.</param>
        void Unsubscribe<TMessage>(Action<TMessage> handler);

        /// <summary>
        /// Publishes a message to all recipients.
        /// </summary>
        /// <typeparam name="TMessage">Type of the published message.</typeparam>
        /// <param name="message">Message that is published.</param>       
        void Publish<TMessage>(TMessage message);

        /// <summary>
        /// Publishes a message to all recipients of the specified type.
        /// </summary>
        /// <typeparam name="TRecipient">Type of the recipient.</typeparam>
        /// <typeparam name="TMessage">Type of the published message.</typeparam>
        /// <param name="message">Message that is published.</param>
        void Publish<TRecipient, TMessage>(TMessage message);

        /// <summary>
        /// Publishes a message instance to all recipients.
        /// </summary>
        /// <param name="message">Message instance.</param>
        void Publish(Object message);
    }
}
