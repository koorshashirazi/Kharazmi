using Newtonsoft.Json;

namespace Kharazmi.AspNetCore.Core.Domain.Events
{
    /// <summary> </summary>
    public class RejectEvent : DomainEvent
    {
        /// <summary> </summary>
        /// <param name="reason"></param>
        [JsonConstructor]
        public RejectEvent(string reason) : base(DomainEventType.From<RejectEvent>())
        {
            Reason = reason;
        }

        public string Reason { get; protected set; }
    }
}