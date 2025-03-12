using System;

namespace Kharazmi.AspNetCore.Core.AuditLogging
{
    /// <summary>
    /// 
    /// </summary>
    public class AuditLogFilter : AuditEvent
    {
        /// <summary>
        /// 
        /// </summary>
        public DateTime? Created { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public new string SubjectName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int PageSize { get; set; } = 10;

        /// <summary>
        /// 
        /// </summary>
        public int Page { get; set; } = 1;
    }
}