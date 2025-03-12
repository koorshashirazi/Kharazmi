using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;

namespace Kharazmi.AspNetCore.Web.Authorization
{
    /// <summary>
    /// 
    /// </summary>
    public class PermissionRequirement : IAuthorizationRequirement
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="permissions"></param>
        public PermissionRequirement(IEnumerable<string> permissions)
        {
            Permissions = permissions;
        }

        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<string> Permissions { get; }

    }
}