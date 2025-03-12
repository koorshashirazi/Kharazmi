using Kharazmi.AspNetCore.Core.GuardToolkit;
using Microsoft.Extensions.DependencyInjection;

namespace Kharazmi.AspNetCore.Web.Localizations
{
    /// <summary>
    /// 
    /// </summary>
    public static class LocalizationServiceExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static IServiceCollection AddLocalizationService(this IServiceCollection services, ResourceOptions options)
        {
            Ensure.ArgumentIsNotNull(options, nameof(options));
            services.AddSingleton(options);
            services.AddSingleton<ISharedLocalizationService, SharedLocalizationService>();
            return services;
        }
    }
}