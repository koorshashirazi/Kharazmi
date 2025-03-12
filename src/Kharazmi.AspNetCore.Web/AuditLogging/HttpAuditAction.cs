using System;
using Kharazmi.AspNetCore.Core.AuditLogging;
using Kharazmi.AspNetCore.Web.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;

namespace Kharazmi.AspNetCore.Web.AuditLogging
{
    /// <summary>
    /// 
    /// </summary>
    public class HttpAuditAction : IAuditAction
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="accessor"></param>
        /// <param name="options"></param>
        public HttpAuditAction(IHttpContextAccessor accessor, AuditHttpActionOptions options)
        {
            if (accessor == null) throw new ArgumentNullException(nameof(accessor));
            if (options == null) throw new ArgumentNullException(nameof(options));
            
            Action = new
            {
                TraceIdentifier = accessor.HttpContext.TraceIdentifier,
                RequestUrl = accessor.HttpContext.Request.GetDisplayUrl(),
                HttpMethod = accessor.HttpContext.Request.Method,
                FormVariables = (options.IncludeFormVariables ? accessor.HttpContext.GetFormVariables() : null)
            };
        }

        public object Action { get; set; }
    }
}