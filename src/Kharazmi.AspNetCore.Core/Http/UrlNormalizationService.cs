using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Kharazmi.AspNetCore.Core.Http
{
    /// <summary>
    /// Url Normalization Service Extensions
    /// </summary>
    public static class UrlNormalizationServiceExtensions
    {
        /// <summary>
        /// Adds IUrlNormalizationService to IServiceCollection.
        /// </summary>
        public static IServiceCollection AddUrlNormalizationService(this IServiceCollection services)
        {
            services.TryAddTransient<IUrlNormalizationService, UrlNormalizationService>();
            return services;
        }
    }

    /// <summary>
    /// Url Normalization Service
    /// </summary>
    public interface IUrlNormalizationService
    {
        /// <summary>
        /// Uses NormalizeUrlAsync method to find the normalized URLs and then compares them.
        /// </summary>
        Task<bool> AreTheSameUrlsAsync(string url1, string url2, bool findRedirectUrl);

        /// <summary>
        /// Uses NormalizeUrlAsync method to find the normalized URLs and then compares them.
        /// </summary>
        Task<bool> AreTheSameUrlsAsync(Uri uri1, Uri uri2, bool findRedirectUrl);

        /// <summary>
        /// URL normalization is the process by which URLs are modified and standardized in a consistent manner. The goal of the normalization process is to transform a URL into a normalized URL so it is possible to determine if two syntactically different URLs may be equivalent.
        /// https://en.wikipedia.org/wiki/URL_normalization
        /// </summary>
        Task<string> NormalizeUrlAsync(Uri uri, bool findRedirectUrl);

        /// <summary>
        /// URL normalization is the process by which URLs are modified and standardized in a consistent manner. The goal of the normalization process is to transform a URL into a normalized URL so it is possible to determine if two syntactically different URLs may be equivalent.
        /// https://en.wikipedia.org/wiki/URL_normalization
        /// </summary>
        Task<string> NormalizeUrlAsync(string url, bool findRedirectUrl);
    }

    /// <summary>
    /// Url Normalization Service
    /// </summary>
    public class UrlNormalizationService : IUrlNormalizationService
    {
        private readonly IRedirectUrlFinderService _locationFinder;

        /// <summary>
        /// Url Normalization Service
        /// </summary>
        public UrlNormalizationService(IRedirectUrlFinderService locationFinder)
        {
            _locationFinder = locationFinder ?? throw new ArgumentNullException(nameof(locationFinder));
        }

        /// <summary>
        /// Uses NormalizeUrlAsync method to find the normalized URLs and then compares them.
        /// </summary>
        public async Task<bool> AreTheSameUrlsAsync(string url1, string url2, bool findRedirectUrl)
        {
            url1 = await NormalizeUrlAsync(url1, findRedirectUrl).ConfigureAwait(false);
            url2 = await NormalizeUrlAsync(url2, findRedirectUrl).ConfigureAwait(false);
            return url1.Equals(url2);
        }

        /// <summary>
        /// Uses NormalizeUrlAsync method to find the normalized URLs and then compares them.
        /// </summary>
        public async Task<bool> AreTheSameUrlsAsync(Uri uri1, Uri uri2, bool findRedirectUrl)
        {
            var url1 = await NormalizeUrlAsync(uri1, findRedirectUrl).ConfigureAwait(false);
            var url2 = await NormalizeUrlAsync(uri2, findRedirectUrl).ConfigureAwait(false);
            return url1.Equals(url2);
        }

        private static readonly string[] DefaultDirectoryIndexes =
        {
            "default.asp",
            "default.aspx",
            "index.htm",
            "index.html",
            "index.php"
        };

        /// <summary>
        /// URL normalization is the process by which URLs are modified and standardized in a consistent manner. The goal of the normalization process is to transform a URL into a normalized URL so it is possible to determine if two syntactically different URLs may be equivalent.
        /// https://en.wikipedia.org/wiki/URL_normalization
        /// </summary>
        public async Task<string> NormalizeUrlAsync(Uri uri, bool findRedirectUrl)
        {
            if (findRedirectUrl)
            {
                uri = await _locationFinder.GetRedirectUrlAsync(uri).ConfigureAwait(false);
            }
            var url = urlToLower(uri);
            url = limitProtocols(url);
            url = removeDefaultDirectoryIndexes(url);
            url = removeTheFragment(url);
            url = removeDuplicateSlashes(url);
            url = addWww(url);
            url = removeFeedburnerPart1(url);
            url = removeFeedburnerPart2(url);
            return removeTrailingSlashAndEmptyQuery(url);
        }

        /// <summary>
        /// URL normalization is the process by which URLs are modified and standardized in a consistent manner. The goal of the normalization process is to transform a URL into a normalized URL so it is possible to determine if two syntactically different URLs may be equivalent.
        /// https://en.wikipedia.org/wiki/URL_normalization
        /// </summary>
        public Task<string> NormalizeUrlAsync(string url, bool findRedirectUrl)
        {
            return NormalizeUrlAsync(new Uri(url), findRedirectUrl);
        }

        private static string removeFeedburnerPart1(string url)
        {
            var idx = url.IndexOf("utm_source=", StringComparison.Ordinal);
            return idx == -1 ? url : url.Substring(0, idx - 1);
        }

        private static string removeFeedburnerPart2(string url)
        {
            var idx = url.IndexOf("utm_medium=", StringComparison.Ordinal);
            return idx == -1 ? url : url.Substring(0, idx - 1);
        }

        private static string addWww(string url)
        {
            if (new Uri(url).Host.Split('.').Length == 2 && !url.Contains("://www."))
            {
                return url.Replace("://", "://www.");
            }
            return url;
        }

        private static string removeDuplicateSlashes(string url)
        {
            var path = new Uri(url).AbsolutePath;
            return path.Contains("//") ? url.Replace(path, path.Replace("//", "/")) : url;
        }

        private static string limitProtocols(string url)
        {
            return new Uri(url).Scheme == "https" ? url.Replace("https://", "http://") : url;
        }

        private static string removeTheFragment(string url)
        {
            var fragment = new Uri(url).Fragment;
            return string.IsNullOrWhiteSpace(fragment) ? url : url.Replace(fragment, string.Empty);
        }

        private static string urlToLower(Uri uri)
        {
            return WebUtility.UrlDecode(uri.AbsoluteUri.ToLowerInvariant());
        }

        private static string removeTrailingSlashAndEmptyQuery(string url)
        {
            return url
                    .TrimEnd('?')
                    .TrimEnd('/');
        }

        private static string removeDefaultDirectoryIndexes(string url)
        {
            foreach (var index in DefaultDirectoryIndexes)
            {
                if (url.EndsWith(index))
                {
                    url = url.TrimEnd(index.ToCharArray());
                    break;
                }
            }
            return url;
        }
    }
}