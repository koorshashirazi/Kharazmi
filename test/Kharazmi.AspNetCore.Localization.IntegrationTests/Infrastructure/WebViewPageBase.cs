using Kharazmi.AspNetCore.Localization.IntegrationTests.Resources;
using Kharazmi.AspNetCore.Localization.Json.Internal;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Razor.Internal;
using Microsoft.Extensions.Localization;

namespace Kharazmi.AspNetCore.Localization.IntegrationTests.Infrastructure
{
    public abstract class WebViewPageBase : WebViewPageBase<dynamic>
    {
    }

    public abstract class WebViewPageBase<TModel> : RazorPage<TModel>
    {
        [RazorInject] public IStringLocalizerFactory LocalizerFactory { get; set; }

        private IStringLocalizer HtmlLocalizer =>
            LocalizerFactory.Create(LocalizationResourceName, LocalizationResourceLocation);

        /// <summary>
        /// The name of the resource to load strings from
        /// It must be set in order to use <see cref="L(string)"/>.
        /// </summary>
        protected string LocalizationResourceName { get; set; } = nameof(MyResource);

        /// <summary>
        /// The location to load resources from
        /// It must be set in order to use <see cref="L(string)"/>.
        /// </summary>
        protected string LocalizationResourceLocation { get; set; } = typeof(MyResource).GetAssemblyFullName();

        /// <summary>
        /// Gets localized string for given key name and current language.
        /// </summary>
        protected string L(string name)
        {
            return HtmlLocalizer.GetString(name);
        }
    }
}
