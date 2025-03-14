﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;

namespace Kharazmi.AspNetCore.Security.Extensions
{
    public static class HttpRequestExtensions
    {
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
        /// Gets the current HttpContext.Request's address.
        /// </summary>
        public static string GetRawUrl(this HttpContext httpContext)
        {
            httpContext.RequestSanityCheck();
            return httpContext.Request.GetDisplayUrl();
        }


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
        public static string GetIp(this HttpContext httpContext, bool tryUseXForwardHeader = true)
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
    }
}