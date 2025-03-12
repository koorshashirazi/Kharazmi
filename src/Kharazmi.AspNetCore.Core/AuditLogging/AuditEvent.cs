using Kharazmi.AspNetCore.Core.Extensions;

namespace Kharazmi.AspNetCore.Core.AuditLogging
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class AuditEvent
    {
        /// <summary>
        /// 
        /// </summary>
        protected AuditEvent()
        {
            Event = GetType().GetGenericTypeName();
        }

        /// <summary>
        /// 
        /// </summary>
        public string Event { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string SubjectIdentifier { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string SubjectName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string SubjectType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public object SubjectAdditionalData { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public object Action { get; set; }
    }
}