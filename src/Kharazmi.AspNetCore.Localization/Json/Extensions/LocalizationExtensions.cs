using System;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Localization;

namespace Kharazmi.AspNetCore.Localization.Json.Extensions
{
    public static class LocalizationExtensions
    {
        public static void AddJsonLocalization(this IServiceCollection services,
            Action<LocalizationOptions> options)
        {
            services.Configure(options);
            services.Add(ServiceDescriptor.Singleton<IStringLocalizerFactory, JsonCacheStringLocalizerFactory>());
            services.AddSingleton<IHtmlLocalizerFactory, JsonHtmlLocalizerFactory>();
            services.Add(ServiceDescriptor.Singleton<ILocalizerService, JsonLocalizationService>());
            services.TryAddTransient(typeof(IStringLocalizer<>), typeof(StringLocalizer<>));
        }

        public static LocalizedString GetString<TResource>(
            this IStringLocalizer stringLocalizer,
            Expression<Func<TResource, string>> propertyExpression)
            => stringLocalizer[(propertyExpression.Body as MemberExpression)?.Member.Name];
    }
}