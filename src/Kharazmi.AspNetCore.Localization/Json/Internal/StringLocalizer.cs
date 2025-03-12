using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Localization;

namespace Kharazmi.AspNetCore.Localization.Json.Internal
{
    internal class StringLocalizer : IStringLocalizer
    {
        private readonly IStringLocalizer _localizer;

        public StringLocalizer(IHostingEnvironment env, IStringLocalizerFactory factory)
        {
            _localizer = factory.Create(string.Empty, Helpers.GetApplicationRoot());
        }

        public LocalizedString this[string name] => _localizer[name];

        public LocalizedString this[string name, params object[] arguments] => _localizer[name, arguments];

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures) => _localizer.GetAllStrings(includeParentCultures);
    }
}