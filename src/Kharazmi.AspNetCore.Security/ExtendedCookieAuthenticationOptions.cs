using Microsoft.AspNetCore.Authentication.Cookies;

namespace Kharazmi.AspNetCore.Security
{
    /// <summary>
    /// 
    /// </summary>
    public class ExtendedCookieAuthenticationOptions : CookieAuthenticationOptions
    {
        /// <summary>
        /// 
        /// </summary>
        public string SessionIdClaim { get; set; }
    }
}