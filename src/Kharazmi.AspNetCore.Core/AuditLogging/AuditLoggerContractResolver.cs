using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Kharazmi.AspNetCore.Core.AuditLogging
{
    /// <summary>
    /// 
    /// </summary>
    public class AuditLoggerContractResolver : DefaultContractResolver
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
            if (typeof(AuditEvent).IsAssignableFrom(member.DeclaringType) &&
                (member.Name == nameof(AuditEvent.Category) ||
                 member.Name == nameof(AuditEvent.SubjectAdditionalData) ||
                 member.Name == nameof(AuditEvent.Action) ||
                 member.Name == nameof(AuditEvent.SubjectIdentifier) || 
                 member.Name == nameof(AuditEvent.SubjectType) ||
                 member.Name == nameof(AuditEvent.SubjectName) ||
                 member.Name == nameof(AuditEvent.Event) ||
                 member.Name == nameof(AuditEvent.Source)))
            {
                property.ShouldSerialize = i => false;
                property.Ignored = true;
            }
            return property;
        }
    }
}