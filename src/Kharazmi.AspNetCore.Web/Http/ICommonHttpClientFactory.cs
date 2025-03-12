using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.Client;

namespace Kharazmi.AspNetCore.Web.Http
{
    /// <summary>
    /// Reusing a single HttpClient instance across a multi-threaded application
    /// </summary>
    public interface ICommonHttpClientFactory : IDisposable
    {
        /// <summary>
        /// Reusing a single HttpClient instance across a multi-threaded application
        /// </summary>
        HttpClient GetOrCreate(
            string baseAddress,
            IDictionary<string, string> defaultRequestHeaders = null,
            TimeSpan? timeout = null,
            long? maxResponseContentBufferSize = null,
            HttpMessageHandler handler = null);
#if NETCOREAPP3_1
        

        Task<DiscoveryDocumentResponse> GetDiscoveryResponseAsync(string baseAddress);

        DiscoveryDocumentResponse GetDiscoveryResponse(string baseAddress);
#endif
#if NETSTANDARD2_0
        /// <summary>
        /// 
        /// </summary>
        /// <param name="baseAddress"></param>
        /// <returns></returns>
        Task<DiscoveryResponse> GetDiscoveryResponseAsync(string baseAddress);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="baseAddress"></param>
        /// <returns></returns>
        DiscoveryResponse GetDiscoveryResponse(string baseAddress);
#endif
    }
}