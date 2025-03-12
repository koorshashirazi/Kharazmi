using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Kharazmi.AspNetCore.Web.Http
{
    /// <summary>
    /// HttpRequest Info
    /// </summary>
    public interface IHttpRequestInfoService
    {
        /// <summary>
        /// Gets the current HttpContext.Request's IP.
        /// </summary>
        string GetIP(bool tryUseXForwardHeader = true);

        /// <summary>
        /// Gets a current HttpContext.Request's header value.
        /// </summary>
        string GetHeaderValue(string headerName);

        /// <summary>
        /// Gets the current HttpContext.Request's UserAgent.
        /// </summary>
        string GetUserAgent();

        /// <summary>
        /// Gets the current HttpContext.Request's Referrer.
        /// </summary>
        string GetReferrerUrl();

        /// <summary>
        /// Gets the current HttpContext.Request's Referrer.
        /// </summary>
        Uri GetReferrerUri();

        /// <summary>
        /// Gets the current HttpContext.Request content's absolute path.
        /// If the specified content path does not start with the tilde (~) character, this method returns contentPath unchanged.
        /// </summary>
        Uri AbsoluteContent(string contentPath);

        /// <summary>
        /// Gets the current HttpContext.Request's root address.
        /// </summary>
        Uri GetBaseUri();

        /// <summary>
        /// Gets the current HttpContext.Request's root address.
        /// </summary>
        string GetBaseUrl();

        /// <summary>
        /// Gets the current HttpContext.Request's address.
        /// </summary>
        string GetRawUrl();

        /// <summary>
        /// Gets the current HttpContext.Request's address.
        /// </summary>
        Uri GetRawUri();

        /// <summary>
        /// Gets the current HttpContext.Request's IUrlHelper.
        /// </summary>
        IUrlHelper GetUrlHelper();

        /// <summary>
        /// Deserialize `request.Body` as a JSON content.
        /// </summary>
        Task<T> DeserializeRequestJsonBodyAsAsync<T>();

        /// <summary>
        /// Reads `request.Body` as string.
        /// </summary>
        Task<string> ReadRequestBodyAsStringAsync();

        /// <summary>
        /// Deserialize `request.Body` as a JSON content.
        /// </summary>
        Task<Dictionary<string, string>> DeserializeRequestJsonBodyAsDictionaryAsync();
    }
}