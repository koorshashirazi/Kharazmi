using System;
using RawRabbit.Common;

namespace Kharazmi.MessageBroker
{
    internal sealed class CustomNamingConventions : NamingConventions
    {
        public CustomNamingConventions(RabbitMqOptions options)
        {
            ExchangeNamingConvention = type => type.GetOrCreateExchangeName(options);
            RoutingKeyConvention = type => type.GetCreateRoutingKey(options);
            QueueNamingConvention = type => type.GetCreateQueueName(options);
            ErrorExchangeNamingConvention = () => ErrorExchangeNaming(options);
            RetryLaterExchangeConvention = span => RetryLaterExchange(span, options);
            RetryLaterQueueNameConvetion = (exchange, span) => RetryLaterQueueName(exchange, span, options);
        }

        internal static string RetryLaterQueueName(string exchange, TimeSpan span, RabbitMqOptions options)
        {
            return $"{options.ExchangeName}.retry_for_{exchange.Replace(".", "_")}_in_{span.TotalMilliseconds}_ms"
                .ToLowerInvariant();
        }

        internal static string RetryLaterExchange(TimeSpan span, RabbitMqOptions options)
        {
            return $"{options.ExchangeName}.retry".ToLowerInvariant();
        }

        internal static string ErrorExchangeNaming(RabbitMqOptions options)
        {
            return $"{options.ExchangeName}.error".ToLowerInvariant();
        }
    }
}