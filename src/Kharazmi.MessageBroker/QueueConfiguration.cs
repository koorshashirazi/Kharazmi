using System.Collections.Generic;

 namespace Kharazmi.MessageBroker
{
    /// <summary></summary>
    public class QueueConfiguration
    {
        /// <summary></summary>
        public string Name { get; set; }

        /// <summary></summary>
        public string NameSuffix { get; set; }

        /// <summary></summary>
        public int PrefetchCount { get; set; }

        /// <summary></summary>
        public bool Durability { get; set; } = true;

        /// <summary></summary>
        public bool AutoDelete { get; set; } = false;

        /// <summary></summary>
        public Dictionary<string, string> Arguments { get; set; }
    }
}