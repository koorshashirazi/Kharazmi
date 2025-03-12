using System;
using RawRabbit.Configuration.Exchange;

namespace Kharazmi.MessageBroker
{
    /// <summary></summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class MessageConfigAttribute : Attribute
    {
        /// <summary></summary>
        public string ExchangeName { get; set; }

        /// <summary></summary>
        public ExchangeType ExchangeType { get; set; }

        /// <summary></summary>
        public string RoutingKey { get; set; }

        /// <summary></summary>
        public string QueueName { get; set; }

        /// <summary></summary>
        public bool Durability { get; set; }

        /// <summary></summary>
        public bool AutoDelete { get; set; }
    }
}