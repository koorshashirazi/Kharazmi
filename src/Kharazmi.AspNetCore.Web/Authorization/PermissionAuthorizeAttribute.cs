using System;
using Kharazmi.AspNetCore.Core.Authorization;
using Kharazmi.AspNetCore.Core.Extensions;
using Microsoft.AspNetCore.Authorization;

namespace Kharazmi.AspNetCore.Web.Authorization
{
    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public class PermissionAuthorizeAttribute : AuthorizeAttribute
    {
        /// <summary>
        /// Creates a new instance of <see cref="AuthorizeAttribute"/> class.
        /// </summary>
        /// <param name="permissions">A list of permissions to authorize</param>
        public PermissionAuthorizeAttribute(params string[] permissions)
        {
            Policy = $"{PermissionConstant.PolicyPrefix}{permissions.PackToString(PermissionConstant.PolicyNameSplitSymbol)}";
        }
    }
}