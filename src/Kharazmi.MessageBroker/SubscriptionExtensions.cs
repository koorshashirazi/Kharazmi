using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Kharazmi.AspNetCore.Core.Domain;

namespace Kharazmi.MessageBroker
{
    /// <summary>
    /// 
    /// </summary>
    public static class SubscriptionExtensions
    {
        private static Assembly _messagesAssembly = Assembly.GetCallingAssembly();

        private static ISet<Type> _excludedMessages = new HashSet<Type>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="subscriber"></param>
        /// <param name="messagesAssembly"></param>
        /// <param name="excludedMessages"></param>
        /// <returns></returns>
        public static IBusSubscriber SubscribeAllMessages(this IBusSubscriber subscriber,
            Assembly messagesAssembly = null,
            ISet<Type> excludedMessages = null)
        {
            _messagesAssembly = messagesAssembly;
            _excludedMessages = excludedMessages;
            return subscriber.SubscribeAllCommands().SubscribeAllEvents();
        }

        private static IBusSubscriber SubscribeAllCommands(this IBusSubscriber subscriber)
            => subscriber.SubscribeAllMessages<IDomainCommand>(nameof(IBusSubscriber.SubscribeCommand));

        private static IBusSubscriber SubscribeAllEvents(this IBusSubscriber subscriber)
            => subscriber.SubscribeAllMessages<IDomainEvent>(nameof(IBusSubscriber.SubscribeEvent));

        private static IBusSubscriber SubscribeAllMessages<TMessage>
            (this IBusSubscriber subscriber, string subscribeMethod)
        {
            var messageTypes = _messagesAssembly
                .GetTypes()
                .Where(t => t.IsClass && typeof(TMessage).IsAssignableFrom(t))
                .Where(t => !_excludedMessages.Contains(t))
                .ToList();

            messageTypes.ForEach(mt => subscriber.GetType().GetMethod(subscribeMethod)?.MakeGenericMethod(mt)
                .Invoke(subscriber,
                    new object[] {mt.GetCustomAttribute<MessageConfigAttribute>()?.ExchangeName, null, null}));

            return subscriber;
        }
    }
}