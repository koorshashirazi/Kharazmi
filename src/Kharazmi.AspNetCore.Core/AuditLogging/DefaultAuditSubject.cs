namespace Kharazmi.AspNetCore.Core.AuditLogging
{
    /// <summary>
    /// 
    /// </summary>
    public class DefaultAuditSubject : IAuditSubject
    {
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
    }
}