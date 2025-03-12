using System;
using Kharazmi.AspNetCore.Core.Extensions;
using Kharazmi.AspNetCore.Core.GuardToolkit;

namespace Kharazmi.AspNetCore.Web.Helpers
{
    /// <summary>
    /// 
    /// </summary>
    public static class UrlHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="requestUri"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public static bool IsLocalUrl(Uri requestUri,  string url)
        {
            Ensure.ArgumentIsNotNull(requestUri, nameof(requestUri));
            Ensure.ArgumentIsNotNull(url, nameof(url));

            return IsRelativeLocalUrl(url) || url.StartsWith(GetLocalUrlRoot(requestUri));
        }

        private static string GetLocalUrlRoot(Uri requestUri)
        {
            var root = requestUri.Scheme + "://" + requestUri.Host;

            if ((string.Equals(requestUri.Scheme, "http", StringComparison.OrdinalIgnoreCase) && requestUri.Port != 80) ||
                (string.Equals(requestUri.Scheme, "https", StringComparison.OrdinalIgnoreCase) && requestUri.Port != 443))
            {
                root += ":" + requestUri.Port;
            }

            return root;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static bool IsRelativeLocalUrl(string url)
        {
            if (url.IsEmpty())  
                return false;
            if (url[0] == 47 && (url.Length == 1 || url[1] != 47 && url[1] != 92))
                return true;
            if (url.Length > 1 && url[0] == 126)
                return url[1] == 47;
            return false;
        }
        
    }
}