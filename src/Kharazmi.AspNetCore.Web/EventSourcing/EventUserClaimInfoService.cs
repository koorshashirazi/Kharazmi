using System.Linq;
using Kharazmi.AspNetCore.Core.EventSourcing;
using Kharazmi.AspNetCore.Core.Extensions;
using Microsoft.AspNetCore.Http;

namespace Kharazmi.AspNetCore.Web.EventSourcing
{
    /// <summary>
    /// 
    /// </summary>
    public class EventUserClaimInfoService : IEventUserClaimInfoService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="accessor"></param>
        /// <param name="options"></param>
        public EventUserClaimInfoService(IHttpContextAccessor accessor, UserClaimInfoOptions options)
        {
            accessor.CheckArgumentIsNull(nameof(accessor));
            options.CheckArgumentIsNull(nameof(options));

            var claims = accessor.HttpContext?.User?
                .GetClaims(options.SpecifiedClaimTypes)
                .Select(x => new UserClaim(x.Type, x.Value)).ToArray();
            
            UserClaimInfo = new UserClaimInfo(claims);
        }

        /// <summary>
        /// 
        /// </summary>
        public UserClaimInfo UserClaimInfo { get; }
    }
}