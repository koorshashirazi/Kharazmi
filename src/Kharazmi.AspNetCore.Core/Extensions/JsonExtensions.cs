using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Kharazmi.AspNetCore.Core.Extensions
{
    public static partial class Core
    {
        public static string StringArrayToJson(this string[] array)
        {
            if (array == null || !array.Any())
                return "[]";
            return JsonConvert.SerializeObject(array);
        }

        public static string ToJson<T>(this T data, bool inlcudeNull = true)
        {
            var setting = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Converters = new JsonConverter[]
                {
                    new StringEnumConverter()
                },
                NullValueHandling = inlcudeNull ? NullValueHandling.Include : NullValueHandling.Ignore
            };

            return JsonConvert.SerializeObject(data, setting);
        }

        public static async Task<dynamic> GetDynamicJsonObject(this Uri uri)
        {
            using (var wc = new WebClient())
            {
                wc.Encoding = Encoding.UTF8;
                wc.Headers["User-Agent"] =
                    "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1; .NET CLR 2.0.50727; .NET4.0C; .NET4.0E)";
                var response = await wc.DownloadStringTaskAsync(uri).ConfigureAwait(false);
                return JsonConvert.DeserializeObject(response);
            }
        }

        public static void AddJsonArrayToList(this string jsonArray , List<string> list)
        {
            if (jsonArray.IsEmpty()) return;
            var listValues = JsonConvert.DeserializeObject<List<string>>(jsonArray);
            if (listValues == null) return;
            list.AddRange(listValues);
        }

        public static string JsonValueToString(this string jsonValue)
        {
            if (jsonValue.IsEmpty()) return null;
            var selectedValue = JsonConvert.DeserializeObject<string>(jsonValue);
            return selectedValue ?? jsonValue;
        }
    }
}