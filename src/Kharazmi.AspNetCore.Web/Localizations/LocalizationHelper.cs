using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kharazmi.AspNetCore.Core.Extensions;
using Kharazmi.AspNetCore.Core.GuardToolkit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

namespace Kharazmi.AspNetCore.Web.Localizations
{
    /// <summary>
    /// 
    /// </summary>
    public static class LocalizationHelper
    {
        /// <summary>
        /// 
        /// </summary>
        public static IStringLocalizerFactory StringLocalizerFactory { set; get; }
        /// <summary>
        /// 
        /// </summary>
        public static ISharedLocalizationService SharedLocalizationService { set; get; }

        /// <summary>
        /// Set CookieRequestCulture , QueryStringRequestCulture
        /// </summary>
        /// <param name="options"></param>
        /// <param name="localizationOptions"></param>
        public static void SetRequestLocalizationOptions(
            this RequestLocalizationOptions options,
            LocalizationOptions localizationOptions)
        {
            Ensure.ArgumentIsNotNull(localizationOptions, nameof(localizationOptions));
            
            var supportedCultures = localizationOptions.SupportedCultures
                .Select(culture => new System.Globalization.CultureInfo(culture)).ToList();

            var defaultCulture = localizationOptions.DefaultSupportedCulture;

            options.DefaultRequestCulture = new RequestCulture(defaultCulture, defaultCulture);
            options.SupportedCultures = supportedCultures;
            options.SupportedUICultures = supportedCultures;


            options.RequestCultureProviders = new List<IRequestCultureProvider>
            {
                new QueryStringRequestCultureProvider(),
                new CustomRequestCultureProvider(context =>
                {
                    var cookieName = localizationOptions.CookieName.IsEmpty()
                        ? CookieRequestCultureProvider.DefaultCookieName
                        : localizationOptions.CookieName;

                    var cultureCookieName = context.Request.Cookies[cookieName];

                    string culture;
                    string uiCulture;
                    if (cultureCookieName.IsNotEmpty())
                    {
                        var cultures = cultureCookieName.Split('|');
                        var c = cultures[0].Replace("c=", "");
                        var uic = cultures[1].Replace("uic=", "");

                        culture = supportedCultures.FirstOrDefault(x => x.Name.Contains(c))?.Name;
                        uiCulture = supportedCultures.FirstOrDefault(x => x.Name.Contains(uic))?.Name;
                    }
                    else
                    {
                        var userLanguage = context.Request.Headers["Accept-Language"].ToString();
                        var firstLang = userLanguage.Split(',').FirstOrDefault();

                        if (firstLang.IsEmpty())
                        {
                            firstLang = defaultCulture;
                        }

                        var userLangCulture = supportedCultures.FirstOrDefault(x => x.Name.Contains(firstLang));

                        culture = userLangCulture != null
                            ? userLangCulture.Name
                            : defaultCulture;

                        uiCulture = userLangCulture != null
                            ? userLangCulture.Name
                            : defaultCulture;
                    }

                    return Task.FromResult(new ProviderCultureResult(culture, uiCulture));
                })
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="app"></param>
        public static void UseLocalizationProvider(this IApplicationBuilder app)
        {
            app.UseRequestLocalization(app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>()
                .Value);

            SharedLocalizationService = app.ApplicationServices.GetService<ISharedLocalizationService>();
            StringLocalizerFactory = app.ApplicationServices.GetService<IStringLocalizerFactory>();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="resourceKey"></param>
        /// <returns></returns>
        public static string GetDisplayName(string resourceKey)
        {
            return SharedLocalizationService == null ? "" : SharedLocalizationService[resourceKey];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="resourceKey"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static string GetDisplayName(string resourceKey, params object[] param)
        {
            return SharedLocalizationService == null ? "" :SharedLocalizationService.GetValueFormatter(resourceKey, param);
        }

        /// <summary>
        /// 
        /// </summary>
        public static System.Globalization.CultureInfo CurrentCulture =>
            System.Threading.Thread.CurrentThread.CurrentCulture;

        /// <summary>
        /// 
        /// </summary>
        public static System.Globalization.CultureInfo CurrentUiCulture =>
            System.Threading.Thread.CurrentThread.CurrentUICulture;

        /// <summary>
        /// 
        /// </summary>
        public static string CurrentCultureName => 
            System.Threading.Thread.CurrentThread.CurrentCulture.Name;
    }
}