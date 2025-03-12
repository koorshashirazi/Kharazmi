using System.Reflection;
using Kharazmi.AspNetCore.Core.Extensions;
using Kharazmi.AspNetCore.Core.GuardToolkit;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.Localization;

namespace Kharazmi.AspNetCore.Web.Localizations
{
    internal class SharedLocalizationService : ISharedLocalizationService
    {
        public IHtmlLocalizer HtmlLocalizer { get; }
        public IStringLocalizer Localizer { get; }

        public SharedLocalizationService(
            IStringLocalizerFactory factory,
            IHtmlLocalizerFactory htmlLocalizerFactory,
            ResourceOptions options)
        {
            var options1 = options;
            Ensure.ArgumentIsNotNull(factory, nameof(factory));
            Ensure.ArgumentIsNotNull(htmlLocalizerFactory, nameof(htmlLocalizerFactory));

            var assemblyName = new AssemblyName(options1.ResourceType.GetTypeInfo().Assembly.FullName);
            Localizer = factory.Create(options1.ResourceType.Name, assemblyName.FullName);
            HtmlLocalizer = htmlLocalizerFactory.Create(options1.ResourceType.Name, assemblyName.FullName);
        }

        public string this[string key] => Localizer[key];

        public LocalizedString GetValue(string key)
        {
            return key.IsNotEmpty() ? Localizer[key] : new LocalizedString("Empty_key", "Empty_Value");
        }

        public LocalizedHtmlString GetValue(string key, params object[] parameter)
        {
            return HtmlLocalizer[key, parameter];
        }

        public string GetValueFormatter(string key, params object[] parameter)
        {
            return key.IsNotEmpty() ? string.Format(GetValue(key), parameter) : string.Format(GetValue(key));
        }

        public LocalizedString GetValueTitle(string key) => GetValue($"{key}Title");
    }
}