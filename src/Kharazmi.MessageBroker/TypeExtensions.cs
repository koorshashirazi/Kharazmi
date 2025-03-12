using System;
using System.Linq;
using System.Reflection;
using Kharazmi.AspNetCore.Core.Domain.Events;
using Kharazmi.AspNetCore.Core.Extensions;
using Kharazmi.AspNetCore.Core.GuardToolkit;
using RawRabbit.Configuration.Exchange;

namespace Kharazmi.MessageBroker
{
    /// <summary>
    /// 
    /// </summary>
    public static class TypeExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static string GetOrCreateExchangeName(this Type type, RabbitMqOptions options)
        {
            Ensure.ArgumentIsNotNull(options, nameof(options));

            var exchangeName = type.GetMessageConfigAttribute()?.ExchangeName;

            var exchange = exchangeName.IsNotEmpty()
                ? exchangeName
                : options.ExchangeName.IsNotEmpty()
                    ? options.ExchangeName
                    : type.Namespace.Underscore().ToLowerInvariant();

            exchange = typeof(RejectEvent).IsAssignableFrom(type)
                ? CustomNamingConventions.ErrorExchangeNaming(options)
                : exchange;

            return exchange;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static string GetCreateRoutingKey(this Type type, RabbitMqOptions options)
        {
            Ensure.ArgumentIsNotNull(options, nameof(options));
            var routingKey = type.GetMessageConfigAttribute()?.RoutingKey;
            var messageNaming = FindMessageNamingConventions(type, options);
            var routingKeyOptions = messageNaming?.RoutingKey;

            routingKey = routingKey.IsNotEmpty()
                ? routingKey
                : routingKeyOptions.IsNotEmpty()
                    ? routingKeyOptions
                    : $"{type.GetOrCreateExchangeName(options)}.{type.Name.Underscore()}".ToLowerInvariant();

            return routingKey;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static string GetCreateQueueName(this Type type, RabbitMqOptions options)
        {
            Ensure.ArgumentIsNotNull(options, nameof(options));

            var queueName = type.GetMessageConfigAttribute()?.QueueName;

            var typeNameOption = FindMessageNamingConventions(type, options);

            var queueNameOptions = typeNameOption?.QueueName;

            queueName = queueName.IsNotEmpty()
                ? queueName
                : queueNameOptions.IsNotEmpty()
                    ? queueNameOptions
                    : $"{type.GetOrCreateExchangeName(options)}.{type.Name.Underscore()}".ToLowerInvariant();

            return queueName;
        }

        /// <summary>
        /// Type must be inherited of Message ConfigAttribute
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static ExchangeType GetExchangeType(this Type type)
        {
            var exchangeType = type.GetMessageConfigAttribute()?.ExchangeType;
            return exchangeType ?? ExchangeType.Topic;
        }

        /// <summary>
        /// Type must be inherited of Message ConfigAttribute
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsDurability(this Type type)
        {
            var durability = type.GetMessageConfigAttribute()?.Durability;
            return durability ?? false;
        }

        /// <summary>
        /// Type must be inherited of Message ConfigAttribute
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsAutoDelete(this Type type)
        {
            var autoDelete = type.GetMessageConfigAttribute()?.AutoDelete;
            return autoDelete ?? false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static MessageNamingConventions FindMessageNamingConventions(this Type type, RabbitMqOptions options)
        {
            return options.MessageNamingConventions?.FirstOrDefault(x =>
                x.TypeFullName.ToLowerInvariant().Equals(type.FullName?.ToLowerInvariant()));
        }

        private static MessageConfigAttribute GetMessageConfigAttribute(this MemberInfo type)
        {
            return type.GetCustomAttribute<MessageConfigAttribute>();
        }
    }
}