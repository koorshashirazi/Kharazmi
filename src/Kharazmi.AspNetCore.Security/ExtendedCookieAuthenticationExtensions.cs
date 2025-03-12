using System;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Kharazmi.AspNetCore.Security
{
    /// <summary>
    /// 
    /// </summary>
    public static class ExtendedCookieAuthenticationExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="authenticationScheme"></param>
        /// <param name="displayName"></param>
        /// <param name="configureOptions"></param>
        /// <returns></returns>
        public static AuthenticationBuilder AddExtendedCookie(this AuthenticationBuilder builder, string authenticationScheme, string displayName, Action<ExtendedCookieAuthenticationOptions> configureOptions)
        {
            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IPostConfigureOptions<ExtendedCookieAuthenticationOptions>, PostConfigureCookieAuthenticationOptions>());
            return builder.AddScheme<ExtendedCookieAuthenticationOptions, ExtendedCookieAuthenticationHandler>(authenticationScheme, displayName, configureOptions);
        }
    }
}