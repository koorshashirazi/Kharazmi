using System.Linq;
using Kharazmi.AspNetCore.Core.AuditLogging;
using Kharazmi.AspNetCore.Core.Extensions;
using Microsoft.AspNetCore.Http;

namespace Kharazmi.AspNetCore.Web.AuditLogging
{
    /// <summary>
    /// 
    /// </summary>
    public class HttpAuditSubject : IAuditSubject
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="accessor"></param>
        /// <param name="options"></param>
        public HttpAuditSubject(IHttpContextAccessor accessor, AuditHttpSubjectOptions options)
        {
            accessor.CheckArgumentIsNull(nameof(accessor));
            
            SubjectIdentifier = accessor.HttpContext.User.FindFirst(options.SubjectIdentifierClaim)?.Value;
            SubjectName = accessor.HttpContext.User.FindFirst(options.SubjectNameClaim)?.Value;
            
            var str1 = accessor.HttpContext.Connection?.RemoteIpAddress?.ToString();
            var str2 = accessor.HttpContext.Connection?.LocalIpAddress?.ToString();
            var claims = accessor.HttpContext.User.Claims;
            
            var datas = claims?.Select(x => new
            {
                Type = x.Type,
                Value = x.Value
            });
            
            SubjectAdditionalData = new
            {
                RemoteIpAddress = str1,
                LocalIpAddress = str2,
                Claims = datas
            };
        }

        /// <summary>
        /// 
        /// </summary>
        public string SubjectName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string SubjectType { get; set; } = "User";

        /// <summary>
        /// 
        /// </summary>
        public object SubjectAdditionalData { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string SubjectIdentifier { get; set; }
    }
}