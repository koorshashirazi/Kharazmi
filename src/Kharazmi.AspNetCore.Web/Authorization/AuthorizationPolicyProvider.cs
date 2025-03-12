using System;
using System.Threading.Tasks;
using Kharazmi.AspNetCore.Core.Authorization;
using Kharazmi.AspNetCore.Core.Common;
using Kharazmi.AspNetCore.Core.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace Kharazmi.AspNetCore.Web.Authorization
{
    /// <summary>
    /// 
    /// </summary>
    public class AuthorizationPolicyProvider : DefaultAuthorizationPolicyProvider
    {
        private readonly LazyConcurrentDictionary<string, AuthorizationPolicy> _policies =
            new LazyConcurrentDictionary<string, AuthorizationPolicy>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        public AuthorizationPolicyProvider(IOptions<AuthorizationOptions> options)
            : base(options)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="policyName"></param>
        /// <returns></returns>
        public override async Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
        {
            if (!policyName.StartsWith(PermissionConstant.PolicyPrefix, StringComparison.OrdinalIgnoreCase))
            {
                return await base.GetPolicyAsync(policyName).ConfigureAwait(false);
            }

            var policy = _policies.GetOrAdd(policyName, name =>
            {
                var permissions = policyName.Substring(PermissionConstant.PolicyPrefix.Length)
                    .UnpackFromString(PermissionConstant.PolicyNameSplitSymbol);

                return new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .AddRequirements(new PermissionRequirement(permissions))
                    .Build();
            });

            return policy;
        }
    }
}