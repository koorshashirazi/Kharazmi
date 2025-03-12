using System.Linq;
using System.Threading.Tasks;
using Kharazmi.AspNetCore.Core.Extensions;
using Microsoft.AspNetCore.Authorization;

namespace Kharazmi.AspNetCore.Web.Authorization
{
    /// <summary> </summary>
    public sealed class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
    {
        /// <summary> </summary>
        public PermissionAuthorizationHandler()
        {
        }
        /// <summary> </summary>
        /// <param name="context"></param>
        /// <param name="requirement"></param>
        /// <returns></returns>
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
            PermissionRequirement requirement)
        {
            if (context.User == null || requirement.Permissions == null || !requirement.Permissions.Any())
                return Task.CompletedTask;

            var hasPermission =
                requirement.Permissions.Any(permission => context.User.HasPermission(permission));

            if (!hasPermission) return Task.CompletedTask;

            context.Succeed(requirement);
            
            return Task.CompletedTask;
        }
    }
}