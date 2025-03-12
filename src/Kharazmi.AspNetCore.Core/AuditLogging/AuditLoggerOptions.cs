using Kharazmi.AspNetCore.Core.Domain.Entities;

namespace Kharazmi.AspNetCore.Core.AuditLogging
{
    public class AuditLoggerOptions
    {
        public string Source { get; set; }
        public TableOption TableOption { get; set; }

        public bool UseDefaultSubject { get; set; } = true;

        public bool UseDefaultAction { get; set; } = true;
    }
}