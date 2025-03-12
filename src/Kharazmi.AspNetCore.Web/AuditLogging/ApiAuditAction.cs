using Kharazmi.AspNetCore.Core.AuditLogging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;

namespace Kharazmi.AspNetCore.Web.AuditLogging
{
    /// <summary>
    /// 
    /// </summary>
    public class ApiAuditAction : IAuditAction
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="accessor"></param>
        public ApiAuditAction(IHttpContextAccessor accessor)
        {
            Action = new
            {
                TraceIdentifier = accessor.HttpContext.TraceIdentifier,
                RequestUrl = accessor.HttpContext.Request.GetDisplayUrl(),
                HttpMethod = accessor.HttpContext.Request.Method
            };
        }

        /// <summary>
        /// 
        /// </summary>
        public object Action { get; set; }
    }
}