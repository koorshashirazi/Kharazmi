using System.Reflection;
using Kharazmi.AspNetCore.Core.AuditLogging;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Kharazmi.AspNetCore.Core.Helpers
{
    public class PrivateSetterContractResolver : DefaultContractResolver
    {
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var prop = base.CreateProperty(member, memberSerialization);
            if (prop.Writable) return prop;
            var property = member as PropertyInfo;
            if (property == null) return prop;
            var hasPrivateSetter = property.GetSetMethod(true) != null;
            prop.Writable = hasPrivateSetter;

            return prop;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public static class JsonSerializerHelper
    {
        static JsonSerializerHelper()
        {
            DefaultJsonSettings.Converters.Add(new StringEnumConverter());
            AuditEventJsonSettings.Converters.Add(new StringEnumConverter());
            AuditEventJsonSettings.ContractResolver = new AuditLoggerContractResolver();
        }


        /// <summary>
        /// 
        /// </summary>
        public static readonly JsonSerializerSettings DefaultJsonSettings = new()
        {
            NullValueHandling = NullValueHandling.Ignore,
            DateFormatHandling = DateFormatHandling.IsoDateFormat,
            Formatting = Formatting.Indented,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            ContractResolver = new PrivateSetterContractResolver()
        };

        /// <summary>
        /// 
        /// </summary>
        public static readonly JsonSerializerSettings AuditEventJsonSettings = new()
        {
            NullValueHandling = NullValueHandling.Ignore,
            DateFormatHandling = DateFormatHandling.IsoDateFormat,
            Formatting = Formatting.Indented
        };

        /// <summary>
        /// 
        /// </summary>
        /// <param name="jsonObject"></param>
        /// <returns></returns>
        public static string Serialize(object jsonObject)
        {
            return JsonConvert.SerializeObject(jsonObject, DefaultJsonSettings);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="jsonObject"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        public static string Serialize(object jsonObject, JsonSerializerSettings settings)
        {
            return JsonConvert.SerializeObject(jsonObject, settings);
        }
    }
}