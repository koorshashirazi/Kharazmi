using System;
using Kharazmi.AspNetCore.Core.Domain.Entities;

namespace Kharazmi.AspNetCore.Core.AuditLogging
{
    /// <summary>
    /// 
    /// </summary>
    public class AuditLog : Entity<int>
    {
        private AuditLog()
        {
            
        }
        public AuditLog(int id) : base(id)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public string Event { get; set; } = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        public string Source { get; set; } = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        public string Category { get; set; } = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        public string SubjectIdentifier { get; set; } = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        public string SubjectName { get; set; } = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        public string SubjectType { get; set; } = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        public string SubjectAdditionalData { get; set; } = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        public string Action { get; set; } = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        public string Data { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime Created { get; set; } = DateTime.UtcNow;
    }
}