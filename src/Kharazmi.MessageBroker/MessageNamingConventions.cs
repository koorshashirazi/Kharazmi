namespace Kharazmi.MessageBroker
{
    /// <summary> </summary>
    public class MessageNamingConventions
    {
        /// <summary> Get or set NameSpace plus type name</summary>
        public string TypeFullName { get; set; }

        /// <summary> </summary>
        public string QueueName { get; set; }

        /// <summary> </summary>
        public string RoutingKey { get; set; }
    }
}