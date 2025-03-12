using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Kharazmi.AspNetCore.Core.GuardToolkit;
using Kharazmi.AspNetCore.Web.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace Kharazmi.AspNetCore.Web.Extensions
{
    public static class HttpRequestExtensions
    {
        private const string RequestedWithHeader = "X-Requested-With";
        private const string XmlHttpRequest = "XMLHttpRequest";

        /// <summary>
        /// Determines whether the specified HTTP request is an AJAX request.
        /// </summary>
        /// <param name="request">The HTTP request.</param>
        /// <returns><c>true</c> if the specified HTTP request is an AJAX request; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="request"/> parameter is <c>null</c>.</exception>
        public static bool IsAjaxRequest(this HttpRequest request)
        {
            Ensure.ArgumentIsNotNull(request, nameof(request));

            return request.Headers != null && request.Headers[RequestedWithHeader] == XmlHttpRequest;
        }

        /// <summary>
        /// Determines whether the specified HTTP request is a local request where the IP address of the request
        /// originator was 127.0.0.1.
        /// </summary>
        /// <param name="request">The HTTP request.</param>
        /// <returns><c>true</c> if the specified HTTP request is a local request; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="request"/> parameter is <c>null</c>.</exception>
        public static bool IsLocalRequest(this HttpRequest request)
        {
            Ensure.ArgumentIsNotNull(request, nameof(request));

            var connection = request.HttpContext.Connection;
            if (connection.RemoteIpAddress != null)
            {
                return connection.LocalIpAddress != null
                    ? connection.RemoteIpAddress.Equals(connection.LocalIpAddress)
                    : IPAddress.IsLoopback(connection.RemoteIpAddress);
            }

            // for in memory TestServer or when dealing with default connection info
            return connection.RemoteIpAddress == null && connection.LocalIpAddress == null;
        }

        /// <summary>
        /// Gets the current HttpContext.Request's UserAgent.
        /// </summary>
        public static string FindUserAgent(this HttpContext httpContext)
        {
            return GetHeaderValue(httpContext, "User-Agent");
        }

        /// <summary>
        /// Gets the current HttpContext.Request's Referrer.
        /// </summary>
        public static string FindReferrerUrl(this HttpContext httpContext)
        {
            return GetHeaderValue(httpContext, "Referer"); // The HTTP referer (originally a misspelling of referrer)
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        public static string GetDisplayUrl(this HttpContext httpContext)
        {
            if (httpContext?.Request == null) return "";

            var request = httpContext.Request;

            string str1 = request.Host.Value ?? "";
            string str2 = request.PathBase != null ? request.PathBase.Value : "";
            string str3 = request.Path != null ? request.Path.Value : "";
            string str4 = request.QueryString.Value ?? "";
            return new StringBuilder(request.Scheme.Length + "://".Length + str1.Length + str2.Length + str3.Length +
                                     str4.Length)
                .Append(request.Scheme)
                .Append("://")
                .Append(str1)
                .Append(str2)
                .Append(str3).Append(str4).ToString();
        }

        /// <summary>
        /// Gets the current HttpContext.Request's Referrer.
        /// </summary>
        public static Uri GetReferrerUri(this HttpContext httpContext)
        {
            var referrer = GetReferrerUrl(httpContext);
            if (string.IsNullOrWhiteSpace(referrer))
            {
                return null;
            }

            return Uri.TryCreate(referrer, UriKind.Absolute, out var result) ? result : null;
        }

        /// <summary>
        /// Gets the current HttpContext.Request's IP.
        /// </summary>
        public static string FindUserIP(this HttpContext httpContext, bool tryUseXForwardHeader = true)
        {
            var ip = string.Empty;

            // todo support new "Forwarded" header (2014) https://en.wikipedia.org/wiki/X-Forwarded-For

            // X-Forwarded-For (csv list):  Using the First entry in the list seems to work
            // for 99% of cases however it has been suggested that a better (although tedious)
            // approach might be to read each IP from right to left and use the first public static IP.
            // http://stackoverflow.com/a/43554000/538763
            //
            if (tryUseXForwardHeader)
            {
                ip = SplitCsv(GetHeaderValue(httpContext, "X-Forwarded-For")).FirstOrDefault();
            }

            // RemoteIpAddress is always null in DNX RC1 Update1 (bug).
            if (string.IsNullOrWhiteSpace(ip) &&
                httpContext?.Connection?.RemoteIpAddress != null)
            {
                ip = httpContext.Connection.RemoteIpAddress.ToString();
            }

            if (string.IsNullOrWhiteSpace(ip))
            {
                ip = GetHeaderValue(httpContext, "REMOTE_ADDR");
            }

            return ip;
        }

        /// <summary>
        /// Gets a current HttpContext.Request's header value.
        /// </summary>
        public static string GetHeaderValue(this HttpContext httpContext, string headerName)
        {
            if (httpContext.Request?.Headers?.TryGetValue(headerName, out var values) ?? false)
            {
                return values.ToString();
            }

            return string.Empty;
        }

        private static IEnumerable<string> SplitCsv(string csvList, bool nullOrWhitespaceInputReturnsNull = false)
        {
            if (string.IsNullOrWhiteSpace(csvList))
            {
                return nullOrWhitespaceInputReturnsNull ? null : new List<string>();
            }

            return csvList
                .TrimEnd(',')
                .Split(',')
                .AsEnumerable()
                .Select(s => s.Trim())
                .ToList();
        }

        /// <summary>
        /// Gets the current HttpContext.Request content's absolute path.
        /// If the specified content path does not start with the tilde (~) character, this method returns contentPath unchanged.
        /// </summary>
        public static Uri AbsoluteContent(this HttpContext httpContext, string contentPath)
        {
            var urlHelper = HttpContextExtensions.GetUrlHelper(httpContext);
            return new Uri(GetBaseUri(httpContext), urlHelper.Content(contentPath));
        }

        /// <summary>
        /// Gets the current HttpContext.Request's root address.
        /// </summary>
        public static Uri GetBaseUri(this HttpContext httpContext)
        {
            return new Uri(GetBaseUrl(httpContext));
        }

        /// <summary>
        /// Gets the current HttpContext.Request's root address.
        /// </summary>
        public static string GetBaseUrl(this HttpContext httpContext)
        {
            RequestSanityCheck(httpContext);
            var request = httpContext.Request;
            return $"{request.Scheme}://{request.Host.ToUriComponent()}";
        }

        /// <summary>
        /// Gets the current HttpContext.Request's address.
        /// </summary>
        public static string GetRawUrl(this HttpContext httpContext)
        {
            RequestSanityCheck(httpContext);
            return httpContext.Request.GetDisplayUrl();
        }

        /// <summary>
        /// Gets the current HttpContext.Request's address.
        /// </summary>
        public static Uri GetRawUri(this HttpContext httpContext)
        {
            return new Uri(GetRawUrl(httpContext));
        }

//        /// <summary>
//        /// Gets the current HttpContext.Request's IUrlHelper.
//        /// </summary>
//        public static IUrlHelper GetUrlHelper(this HttpContext httpContext)
//        {
//            var urlHelper = httpContext.RequestServices.GetService<IUrlHelper>();
//            return urlHelper ?? throw new NullReferenceException(nameof(urlHelper));
//        }

        private static void RequestSanityCheck(this HttpContext httpContext)
        {
            if (httpContext == null)
            {
                throw new NullReferenceException("HttpContext is null.");
            }

            if (httpContext.Request == null)
            {
                throw new NullReferenceException("HttpContext.Request is null.");
            }
        }


        /// <summary>
        /// Gets the current HttpContext.Request's UserAgent.
        /// </summary>
        public static string GetUserAgent(this HttpContext httpContext)
        {
            return httpContext.GetHeaderValue("User-Agent");
        }

        /// <summary>
        /// Gets the current HttpContext.Request's Referrer.
        /// </summary>
        public static string GetReferrerUrl(this HttpContext httpContext)
        {
            return httpContext.GetHeaderValue("Referer"); // The HTTP referer (originally a misspelling of referrer)
        }

        /// <summary>
        /// Gets the current HttpContext.Request's IP.
        /// </summary>
        public static string GetIP(this HttpContext httpContext, bool tryUseXForwardHeader = true)
        {
            string ip = string.Empty;

            // todo support new "Forwarded" header (2014) https://en.wikipedia.org/wiki/X-Forwarded-For

            // X-Forwarded-For (csv list):  Using the First entry in the list seems to work
            // for 99% of cases however it has been suggested that a better (although tedious)
            // approach might be to read each IP from right to left and use the first public static IP.
            // http://stackoverflow.com/a/43554000/538763
            //
            if (tryUseXForwardHeader)
            {
                ip = SplitCsv(httpContext.GetHeaderValue("X-Forwarded-For")).FirstOrDefault();
            }

            // RemoteIpAddress is always null in DNX RC1 Update1 (bug).
            if (string.IsNullOrWhiteSpace(ip) &&
                httpContext?.Connection?.RemoteIpAddress != null)
            {
                ip = httpContext.Connection.RemoteIpAddress.ToString();
            }

            if (string.IsNullOrWhiteSpace(ip))
            {
                ip = httpContext.GetHeaderValue("REMOTE_ADDR");
            }

            return ip;
        }


        /// <summary>
        /// Gets the current HttpContext.Request's IUrlHelper.
        /// </summary>
        public static IUrlHelper GetUrlHelper(this HttpContext httpContext)
        {
            return httpContext.RequestServices.GetRequiredService<IUrlHelper>();
        }

        private static void requestSanityCheck(this HttpContext httpContext)
        {
            if (httpContext == null)
            {
                throw new NullReferenceException("HttpContext is null.");
            }

            if (httpContext.Request == null)
            {
                throw new NullReferenceException("HttpContext.Request is null.");
            }
        }

        /// <summary>
        /// Deserialize `request.Body` as a JSON content.
        /// </summary>
        public static async Task<T> DeserializeRequestJsonBodyAsAsync<T>(this HttpContext httpContext)
        {
            requestSanityCheck(httpContext);
            var request = httpContext.Request;
            using (var bodyReader = new StreamReader(request.Body, Encoding.UTF8))
            {
                var body = await bodyReader.ReadToEndAsync().ConfigureAwait(false);
                request.Body = new MemoryStream(Encoding.UTF8.GetBytes(body));
#if !NETCOREAPP3_0
                return JsonConvert.DeserializeObject<T>(body);
#else
                return JsonSerializer.Deserialize<T>(body);
#endif
            }
        }

        /// <summary>
        /// Reads `request.Body` as string.
        /// </summary>
        public static async Task<string> ReadRequestBodyAsStringAsync(this HttpContext httpContext)
        {
            requestSanityCheck(httpContext);
            var request = httpContext.Request;
            using var bodyReader = new StreamReader(request.Body, Encoding.UTF8);
            var body = await bodyReader.ReadToEndAsync().ConfigureAwait(false);
            request.Body = new MemoryStream(Encoding.UTF8.GetBytes(body));
            return body;
        }

        /// <summary>
        /// Deserialize `request.Body` as a JSON content.
        /// </summary>
        public static async Task<Dictionary<string, string>> DeserializeRequestJsonBodyAsDictionaryAsync(
            this HttpContext httpContext)
        {
            requestSanityCheck(httpContext);
            var request = httpContext.Request;
            using var bodyReader = new StreamReader(request.Body, Encoding.UTF8);
            var body = await bodyReader.ReadToEndAsync().ConfigureAwait(false);
            request.Body = new MemoryStream(Encoding.UTF8.GetBytes(body));
#if !NETCOREAPP3_0
            return JsonConvert.DeserializeObject<Dictionary<string, string>>(body);
#else
                return JsonSerializer.Deserialize<Dictionary<string, string>>(body);
#endif
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static string GetOrigin(this HttpRequest request)
        {
            if (request.Headers.TryGetValue("Origin", out var origin))
            {
                return origin;
            }

            return "";
        }
    }
}