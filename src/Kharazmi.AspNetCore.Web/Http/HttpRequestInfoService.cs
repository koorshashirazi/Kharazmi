using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Kharazmi.AspNetCore.Web.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Kharazmi.AspNetCore.Web.Http
{
    /// <summary>
    /// Http Request Info
    /// </summary>
    public class HttpRequestInfoService : IHttpRequestInfoService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUrlHelper _urlHelper;

        /// <summary>
        /// Http Request Info
        /// </summary>
        public HttpRequestInfoService(IHttpContextAccessor httpContextAccessor, IUrlHelper urlHelper)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _urlHelper = urlHelper;
        }

        /// <summary>
        /// Gets the current HttpContext.Request's UserAgent.
        /// </summary>
        public string GetUserAgent()
        {
            return GetHeaderValue("User-Agent");
        }

        /// <summary>
        /// Gets the current HttpContext.Request's Referrer.
        /// </summary>
        public string GetReferrerUrl()
        {
            return _httpContextAccessor.HttpContext.GetReferrerUrl();
        }

        /// <summary>
        /// Gets the current HttpContext.Request's Referrer.
        /// </summary>
        public Uri GetReferrerUri()
        {
            return HttpRequestExtensions.GetReferrerUri(_httpContextAccessor.HttpContext);
        }

        /// <summary>
        /// Gets the current HttpContext.Request's IP.
        /// </summary>
        public string GetIP(bool tryUseXForwardHeader = true)
        {
            return _httpContextAccessor.HttpContext.FindUserIP(tryUseXForwardHeader);
        }

        /// <summary>
        /// Gets a current HttpContext.Request's header value.
        /// </summary>
        public string GetHeaderValue(string headerName)
        {
            return HttpRequestExtensions.GetHeaderValue(_httpContextAccessor.HttpContext, headerName);
        }

        /// <summary>
        /// Gets the current HttpContext.Request content's absolute path.
        /// If the specified content path does not start with the tilde (~) character, this method returns contentPath unchanged.
        /// </summary>
        public Uri AbsoluteContent(string contentPath)
        {
            var urlHelper = _urlHelper ?? throw new NullReferenceException(nameof(_urlHelper));
            return new Uri(GetBaseUri(), urlHelper.Content(contentPath));
        }

        /// <summary>
        /// Gets the current HttpContext.Request's root address.
        /// </summary>
        public Uri GetBaseUri()
        {
            return new Uri(GetBaseUrl());
        }

        /// <summary>
        /// Gets the current HttpContext.Request's root address.
        /// </summary>
        public string GetBaseUrl()
        {
            return HttpRequestExtensions.GetBaseUrl(_httpContextAccessor.HttpContext);
        }

        /// <summary>
        /// Gets the current HttpContext.Request's address.
        /// </summary>
        public string GetRawUrl()
        {
            return HttpRequestExtensions.GetRawUrl(_httpContextAccessor.HttpContext);
        }

        /// <summary>
        /// Gets the current HttpContext.Request's address.
        /// </summary>
        public Uri GetRawUri()
        {
            return new Uri(GetRawUrl());
        }

        /// <summary>
        /// Gets the current HttpContext.Request's IUrlHelper.
        /// </summary>
        public IUrlHelper GetUrlHelper()
        {
            return _urlHelper ?? throw new NullReferenceException(nameof(_urlHelper));
        }

        /// <summary>
        /// Deserialize `request.Body` as a JSON content.
        /// </summary>
        public Task<T> DeserializeRequestJsonBodyAsAsync<T>()
        {
            return HttpRequestExtensions.DeserializeRequestJsonBodyAsAsync<T>(_httpContextAccessor.HttpContext);
        }

        /// <summary>
        /// Reads `request.Body` as string.
        /// </summary>
        public Task<string> ReadRequestBodyAsStringAsync()
        {
            return HttpRequestExtensions.ReadRequestBodyAsStringAsync(_httpContextAccessor.HttpContext);
        }

        /// <summary>
        /// Deserialize `request.Body` as a JSON content.
        /// </summary>
        public Task<Dictionary<string, string>> DeserializeRequestJsonBodyAsDictionaryAsync()
        {
            return HttpRequestExtensions.DeserializeRequestJsonBodyAsDictionaryAsync(_httpContextAccessor.HttpContext);
        }
    }
}