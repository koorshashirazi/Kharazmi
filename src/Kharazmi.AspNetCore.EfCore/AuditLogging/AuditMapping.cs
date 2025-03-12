using Kharazmi.AspNetCore.Core.AuditLogging;
using Kharazmi.AspNetCore.Core.Helpers;

namespace Kharazmi.AspNetCore.EFCore.AuditLogging
{
    public static class AuditMapping
    {
        public static TAuditLog MapToEntity<TAuditLog>(this AuditEvent auditEvent) where TAuditLog : AuditLog, new()
        {
            var auditLog = new TAuditLog
            {
                Event = auditEvent.Event,
                Source = auditEvent.Source,
                SubjectIdentifier = auditEvent.SubjectIdentifier,
                SubjectName = auditEvent.SubjectName,
                SubjectType = auditEvent.SubjectType,
                Category = auditEvent.Category,
                Data = JsonSerializerHelper.Serialize(auditEvent, JsonSerializerHelper.AuditEventJsonSettings),
                Action = auditEvent.Action == null ? null : JsonSerializerHelper.Serialize(auditEvent.Action),
                SubjectAdditionalData = auditEvent.SubjectAdditionalData == null
                    ? null
                    : JsonSerializerHelper.Serialize(auditEvent.SubjectAdditionalData)
            };
            return auditLog;
        }
    }
}