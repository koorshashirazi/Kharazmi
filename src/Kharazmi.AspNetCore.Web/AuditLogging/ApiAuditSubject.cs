﻿using System.Linq;
using Kharazmi.AspNetCore.Core.AuditLogging;
using Microsoft.AspNetCore.Http;

namespace Kharazmi.AspNetCore.Web.AuditLogging
{
    /// <summary>
    /// 
    /// </summary>
    public class ApiAuditSubject : IAuditSubject
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="accessor"></param>
        /// <param name="auditLoggingConfiguration"></param>
        public ApiAuditSubject(IHttpContextAccessor accessor, AuditLoggingConfiguration auditLoggingConfiguration)
        {
            var subClaim = accessor.HttpContext.User.FindFirst(auditLoggingConfiguration.SubjectIdentifierClaim);
            var nameClaim = accessor.HttpContext.User.FindFirst(auditLoggingConfiguration.SubjectNameClaim);
            var clientIdClaim = accessor.HttpContext.User.FindFirst(auditLoggingConfiguration.ClientIdClaim);

            SubjectIdentifier = subClaim == null ? clientIdClaim.Value : subClaim.Value;
            SubjectName = subClaim == null ? clientIdClaim.Value : nameClaim?.Value;
            SubjectType = subClaim == null ? AuditSubjectTypes.Machine : AuditSubjectTypes.User;

            SubjectAdditionalData = new
            {
                RemoteIpAddress = accessor.HttpContext.Connection?.RemoteIpAddress?.ToString(),
                LocalIpAddress = accessor.HttpContext.Connection?.LocalIpAddress?.ToString(),
                Claims = accessor.HttpContext.User.Claims?.Select(x => new { x.Type, x.Value })
            };
        }

        public string SubjectName { get; set; }

        public string SubjectType { get; set; }

        public object SubjectAdditionalData { get; set; }

        public string SubjectIdentifier { get; set; }
    }
}