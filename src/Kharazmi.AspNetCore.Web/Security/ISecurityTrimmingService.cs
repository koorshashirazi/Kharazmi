using Kharazmi.AspNetCore.Core.Functional;
using Microsoft.AspNetCore.Http;

namespace Kharazmi.AspNetCore.Web.Security
{
    /// <summary>
    /// 
    /// </summary>
    public interface ISecurityTrimmingService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="area"></param>
        /// <param name="controller"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        Result CanCurrentUserAccess(string area, string controller, string action);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="httpContextAccessor"></param>
        /// <param name="area"></param>
        /// <param name="controller"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        Result CanUserAccess(IHttpContextAccessor httpContextAccessor, string area, string controller,
            string action);
    }
}