using System;

 namespace Kharazmi.AspNetCore.Localization
{
    public static class DefaultConfiguration
    {
        public static string AppDomain() => $"{AppContext.BaseDirectory}/Localization/";
        public const string LocalizationCacheKeyTemplate = "CULTURE_{0}_RESOURCE_{1}_KEY_{2}_CACHE_KEY";
        public const string LocalizationPathTemplate = "CULTURE_{0}_RESOURCE_{1}";
    }
}
