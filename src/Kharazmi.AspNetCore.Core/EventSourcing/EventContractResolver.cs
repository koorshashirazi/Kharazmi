using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Kharazmi.AspNetCore.Core.EventSourcing
{
    /// <summary>
    /// 
    /// </summary>
    public class EventContractResolver : DefaultContractResolver
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="member"></param>
        /// <param name="memberSerialization"></param>
        /// <returns></returns>
        protected override JsonProperty CreateProperty(
            MemberInfo member,
            MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);
            // if (typeof(IDomainEvent).IsAssignableFrom(member.DeclaringType) &&
            //      member.AggregateType == nameof(DomainEvent.OccurrendOn) ||
            //      member.AggregateType == nameof(DomainEvent.Action))
            // {
            //     property.ShouldSerialize = i => false;
            //     property.Ignored = true;
            // }
            return property;
        }
        
    }
}