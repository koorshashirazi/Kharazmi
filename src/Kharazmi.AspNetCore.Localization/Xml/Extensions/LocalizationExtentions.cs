using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

namespace Kharazmi.AspNetCore.Localization.Xml.Extensions
{
    public static class LocalizationExtentions
    {
        public static void AddXmlLocalization(this IServiceCollection services,
            Action<LocalizationOptions> options)
        {
            services.Configure(options);
            services.Add(ServiceDescriptor.Singleton<IStringLocalizerFactory, XmlStringLocalizerFactory>());
            services.Add(ServiceDescriptor.Singleton<ILocalizerService, XmlLocalizationService>());
        }
    }
}
