using System;
using System.Collections.Generic;
using System.Globalization;
using Kharazmi.AspNetCore.Localization.Json.Internal;
using Microsoft.Extensions.Localization;

namespace Kharazmi.AspNetCore.Localization
{
    public abstract class BaseStringLocalization : IStringLocalizer
    {
        public CultureInfo Culture { get; } = CultureInfo.CurrentUICulture;
        public string ResourcePath { get; }
        public string ResourceName { get; }

        protected BaseStringLocalization(
            string resourcePath,
            string resourceName)
        {
            ResourcePath = resourcePath;
            ResourceName = resourceName;
        }

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures) =>
            throw new NotImplementedException(nameof(GetAllStrings));

        public IStringLocalizer WithCulture(CultureInfo culture) =>
            throw new NotImplementedException(nameof(WithCulture));

        public LocalizedString this[string name] => GetValue(name);

        public LocalizedString this[string name, params object[] arguments] => GetValue(name, arguments);

        protected LocalizedString GetValue(string name, params object[] arguments)
        {
            var computedKey = GetComputedKey(name);
            var value = string.Format(computedKey ?? name, arguments);

            return new LocalizedString(name, value, value.IsEmpty());
        }


        protected virtual string GetComputedKey(string name)
        {
            var computedKey = Helpers.GetComputedCacheKey(ResourceName, name, Culture.Name);
            return computedKey;
        }

        protected virtual string GetComputedPath(string culture)
        {
            var computedPath = Helpers.GetComputedResourceFile(ResourcePath, ResourceName, culture);
            return computedPath;
        }
    }
}