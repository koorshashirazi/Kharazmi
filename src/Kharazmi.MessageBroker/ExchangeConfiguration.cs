using System.Collections.Generic;
using RawRabbit.Configuration.Exchange;

namespace Kharazmi.MessageBroker
{
    /// <summary></summary>
    public class ExchangeConfiguration
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
        public string ConsumerTag { get; set; }

        /// <summary></summary>
        public ushort PrefetchCount { get; set; }

        /// <summary></summary>
        public bool Durability { get; set; }

        /// <summary></summary>
        public bool AutoDelete { get; set; } = false;

        /// <summary></summary>
        public bool AutoAck { get; set; } = true;

        /// <summary></summary>
        public bool NoLocal { get; set; } = true;

        /// <summary></summary>
        public Dictionary<string, string> Arguments { get; set; }
    }
}