using Kharazmi.AspNetCore.Core.EventSourcing;
using Kharazmi.AspNetCore.Core.Extensions;
using Kharazmi.AspNetCore.Core.GuardToolkit;
using Microsoft.AspNetCore.Http;

namespace Kharazmi.AspNetCore.Web.EventSourcing
{
    /// <summary>
    /// 
    /// </summary>
    public class EventUserInfoService : IEventUserInfoService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="httpContextAccessor"></param>
        /// <param name="options"></param>
        public EventUserInfoService(
            IHttpContextAccessor httpContextAccessor,
            UserInfoOptions options)
        {
            Ensure.ArgumentIsNotNull(httpContextAccessor, nameof(httpContextAccessor));
            var user = httpContextAccessor.HttpContext?.User;

            if (user is null)
            {
                UserInfo = new UserInfo(false, null, null);
                return;
            }

            UserInfo = new UserInfo(user.Identity.IsAuthenticated, user.FindFirstValue(options.UserIdClaimType),
                user.FindFirstValue(options.UserNameClaimType));
        }

        /// <summary>
        /// 
        /// </summary>
        public UserInfo UserInfo { get; }
    }
}