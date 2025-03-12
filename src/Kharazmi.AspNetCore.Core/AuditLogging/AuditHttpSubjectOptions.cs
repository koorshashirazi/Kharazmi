namespace Kharazmi.AspNetCore.Core.AuditLogging
{
    /// <summary>
    /// 
    /// </summary>
    public class AuditHttpSubjectOptions
    {
        /// <summary>
        /// 
        /// </summary>
        public string SubjectIdentifierClaim { get; set; } = "sub";

        /// <summary>
        /// 
        /// </summary>
        public string SubjectNameClaim { get; set; } = "name";
    }
}