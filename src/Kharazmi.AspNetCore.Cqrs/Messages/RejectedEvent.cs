using Newtonsoft.Json;

namespace Kharazmi.AspNetCore.Cqrs.Messages
{
    /// <summary> </summary>
    public class RejectedEvent : DomainEvent
    {
        /// <summary> </summary>
        /// <param name="reason"></param>
        [JsonConstructor]
        public RejectedEvent(string reason)
        {
            Reason = reason;
        }
        /// <summary>  </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static RejectedEvent For(string name)
            => new RejectedEvent($"There was an error when executing: {name}");
    }
}