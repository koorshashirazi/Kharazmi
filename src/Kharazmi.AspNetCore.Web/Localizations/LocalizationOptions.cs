using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Kharazmi.AspNetCore.Web.Localizations
{
    /// <summary>
    /// 
    /// </summary>
    public class LocalizationOptions
    {
        /// <summary>
        /// 
        /// </summary>
        public LocalizationOptions()
        {
            SupportedCultures = new List<string>();
        }

        /// <summary>
        /// 
        /// </summary>
        public string CookieName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string DefaultSupportedCulture { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<string> SupportedCultures { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string DateFormatString { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        [JsonProperty(TypeNameHandling = TypeNameHandling.All)]
        public TimeSpan DefaultCultureCookieExpires { get; set; }
    }
}