namespace Kharazmi.AspNetCore.Core.AuditLogging
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class AuditLoggingConfiguration
    {
        /// <summary>
        /// 
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string SubjectIdentifierClaim { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string SubjectNameClaim { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string ClientIdClaim { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public bool IncludeFormVariables { get; set; }
    }
}