using System;
using Kharazmi.AspNetCore.Core.Domain.Entities;

// ReSharper disable InconsistentNaming

namespace Kharazmi.AspNetCore.Core.Logging
{
    /// <summary>
    /// Represents a log in the logging database.
    /// </summary>
    public class Log : Entity<Guid>
    {
        public Log()
        {
            
        }
        public Log(Guid id) : base(id)
        {
        }

        public string Message { get; set; } = string.Empty;
        public string Level { get; set; } = string.Empty;
        public DateTime CreationTime { get; set; } = DateTime.UtcNow;
        public string LoggerName { get; set; } = string.Empty;
        public string UserBrowserName { get; set; } = string.Empty;
        public string UserIP { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string UserDisplayName { get; set; } = string.Empty;
        public string TenantId { get; set; } = string.Empty;
        public string TenantName { get; set; } = string.Empty;
        public string ImpersonatorUserId { get; set; } = string.Empty;
        public string ImpersonatorTenantId { get; set; } = string.Empty;
        public int EventId { get; set; }
    }
}