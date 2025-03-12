namespace Kharazmi.AspNetCore.Core.AuditLogging
{
    /// <summary>
    /// 
    /// </summary>
    public interface IAuditSubject
    {
        /// <summary>
        /// 
        /// </summary>
        string SubjectIdentifier { get; set; }

        /// <summary>
        /// 
        /// </summary>
        string SubjectName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        string SubjectType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        object SubjectAdditionalData { get; set; }
    }
}