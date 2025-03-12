using System;
using Kharazmi.AspNetCore.Core.Extensions;
using Kharazmi.AspNetCore.Core.GuardToolkit;
using Kharazmi.AspNetCore.Core.Serialization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Kharazmi.AspNetCore.Security
{
    /// <summary>
    /// 
    /// </summary>
    public class AuthenticationDistributeCacheDataFormatOptions
    {
        /// <summary>
        /// 
        /// </summary>
        public string AuthenticationPropertyPurpose { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string SecureDataFormatCacheKeyPrefix { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public TimeSpan DefaultCacheDuration { get; set; }
    }

    /// <summary>
    /// Custom secure data format intended for use with OpenIdConnectMiddleware.
    /// It is intended to reduce the size of the state parameter generated
    /// by the default PropertiesDataFormat, which results in query strings
    /// that are too long for Azure AD to handle.
    /// </summary>
    public class AuthenticationDistributeCacheDataFormat : ISecureDataFormat<AuthenticationProperties>
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly AuthenticationDistributeCacheDataFormatOptions _options;
        private readonly string _name;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticationDistributeCacheDataFormat"/> class.
        /// </summary>
        /// <param name="serviceProvider">The HTTP context </param>
        /// <param name="options"></param>
        /// <param name="name">The scheme name.</param>
        public AuthenticationDistributeCacheDataFormat(
            IServiceProvider serviceProvider,
            AuthenticationDistributeCacheDataFormatOptions options,
            string name)
        {
            _serviceProvider = Ensure.IsNotNullWithDetails(serviceProvider, nameof(serviceProvider));
            _options = Ensure.IsNotNullWithDetails(options, nameof(options));
            _name = name;
        }

        private IDistributedCache Cache => 
            _serviceProvider.GetRequiredService<IDistributedCache>();

        private IDataProtectionProvider Protector =>
            _serviceProvider.GetRequiredService<IDataProtectionProvider>();

        /// <summary>
        /// Protects the specified data.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        public string Protect(AuthenticationProperties data)
        {
            return Protect(data, _options.AuthenticationPropertyPurpose);
        }

        /// <summary>
        /// Protects the specified data.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="purpose">The purpose.</param>
        /// <returns></returns>
        public string Protect(AuthenticationProperties data, string purpose)
        {
            var key = $"{Guid.NewGuid():N}";
            var cacheKey = $"{_options.SecureDataFormatCacheKeyPrefix}{purpose}_{key}";
            var json = ObjectSerializer.ToString(data);

            var options = new DistributedCacheEntryOptions();
            if (data.ExpiresUtc.HasValue)
                options.SetAbsoluteExpiration(data.ExpiresUtc.Value);
            else
                options.SetAbsoluteExpiration(_options.DefaultCacheDuration);

            // Rather than encrypt the full AuthenticationProperties
            // cache the data and encrypt the key that points to the data
            Cache.SetString(cacheKey, json, options);

            return Protector.CreateProtector(purpose, _name, "v2").Protect(key);
        }

        /// <summary>
        /// Unprotects the specified protected text.
        /// </summary>
        /// <param name="protectedText">The protected text.</param>
        /// <returns></returns>
        public AuthenticationProperties Unprotect(string protectedText)
        {
            return Unprotect(protectedText, _options.AuthenticationPropertyPurpose);
        }

        /// <summary>
        /// Unprotects the specified protected text.
        /// </summary>
        /// <param name="protectedText">The protected text.</param>
        /// <param name="purpose">The purpose.</param>
        /// <returns></returns>
        public AuthenticationProperties Unprotect(string protectedText, string purpose)
        {
            if (protectedText.IsEmpty())
                return null;

            // Decrypt the key and retrieve the data from the cache.
            var key = Protector.CreateProtector(purpose, _name, "v2").Unprotect(protectedText);
            var cacheKey = $"{_options.SecureDataFormatCacheKeyPrefix}{purpose}_{key}";
            var json = Cache.GetString(cacheKey);

            return json == null ? null : ObjectSerializer.FromString<AuthenticationProperties>(json);
        }
    }

    internal static class ObjectSerializer
    {
        private static readonly JsonSerializerSettings _settings = new JsonSerializerSettings
        {
            DefaultValueHandling = DefaultValueHandling.Ignore,
            NullValueHandling = NullValueHandling.Ignore
        };

        private static readonly JsonSerializer _serializer = new JsonSerializer
        {
            DefaultValueHandling = DefaultValueHandling.Ignore,
            NullValueHandling = NullValueHandling.Ignore
        };

        static ObjectSerializer()
        {
            _settings.Converters.Add(new NameValueCollectionConverter());
        }

        public static string ToString(object o)
        {
            return JsonConvert.SerializeObject(o, _settings);
        }

        public static T FromString<T>(string value)
        {
            return JsonConvert.DeserializeObject<T>(value, _settings);
        }

        public static JObject ToJObject(object o)
        {
            return JObject.FromObject(o, _serializer);
        }
    }
}