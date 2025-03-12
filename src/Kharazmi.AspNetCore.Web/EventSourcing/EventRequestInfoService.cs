using Kharazmi.AspNetCore.Core.EventSourcing;
using Kharazmi.AspNetCore.Core.Extensions;
using Kharazmi.AspNetCore.Web.Extensions;
using Kharazmi.AspNetCore.Web.Http;
using Microsoft.AspNetCore.Http;

namespace Kharazmi.AspNetCore.Web.EventSourcing
{
    /// <summary>
    /// 
    /// </summary>
    internal class EventRequestInfoService : IEventRequestInfoService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="accessor"></param>
        /// <param name="options"></param>
        public EventRequestInfoService(
            IHttpContextAccessor accessor,
            RequestInfoOptions options)
        {
            accessor.CheckArgumentIsNull(nameof(accessor));
            options.CheckArgumentIsNull(nameof(options));

            var httpContext = accessor.HttpContext;
            if (httpContext == null)
            {
                RequestInfo = new RequestInfo();
                return;
            }
            
            RequestInfo = new RequestInfo
            {
                TraceIdentifier = httpContext.TraceIdentifier,
                UriValue = httpContext.GetDisplayUrl(),
                HttpMethod = httpContext.Request?.Method ?? "",
                Agent = httpContext.FindUserAgent() ?? "",
                RemoteIpAddress = httpContext.FindUserIP() ?? "",
                LocalIpAddress = httpContext.Connection?.LocalIpAddress?.ToString(),
                FormVariables = options.IncludeFormVariables ? httpContext.GetFormVariables() : null
            };
        }

        /// <summary>
        /// 
        /// </summary>
        public RequestInfo RequestInfo { get; }
    }
}