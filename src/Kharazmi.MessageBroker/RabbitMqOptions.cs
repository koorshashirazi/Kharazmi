using System.Collections.Generic;
using RawRabbit.Configuration;

namespace Kharazmi.MessageBroker
{
    /// <summary> </summary>
    public class RabbitMqOptions : RawRabbitConfiguration
    {
        /// <summary> </summary>
        public string ExchangeName { get; set; }

        /// <summary> </summary>
        public int Retries { get; set; }

        /// <summary> </summary>
        public int RetryInterval { get; set; }

        /// <summary> </summary>
        public bool WithRequeuing { get; set; }

        /// <summary> </summary>
        public List<MessageNamingConventions> MessageNamingConventions { get; set; }
    }
}