using Kharazmi.AspNetCore.Core.Runtime;
using Microsoft.AspNetCore.Authorization;

namespace Kharazmi.AspNetCore.Web.Authorization
{
    /// <summary>
    /// 
    /// </summary>
    public static class AuthorizationExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        public static void AddHeadOfficeOnlyPolicy(this AuthorizationOptions options)
        {
            options.AddPolicy(PolicyNames.HeadOfficeOnly,
                policy => policy.RequireClaim(UserClaimTypes.IsHeadOffice, "true"));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        public static void AddHeadTenantOnlyPolicy(this AuthorizationOptions options)
        {
            options.AddPolicy(PolicyNames.HeadTenantOnly,
                policy => policy.RequireClaim(UserClaimTypes.IsHeadTenant, "true"));
        }
    }
}