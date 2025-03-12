using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Newtonsoft.Json;

namespace Kharazmi.AspNetCore.Web.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    public static class TempDataExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tempData"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <typeparam name="T"></typeparam>
        public static void Set<T>(this ITempDataDictionary tempData, string key, T value) where T : class
        {
            tempData[key] = JsonConvert.SerializeObject(value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tempData"></param>
        /// <param name="key"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T Get<T>(this ITempDataDictionary tempData, string key) where T : class
        {
            object o;
            tempData.TryGetValue(key, out o);
            return o == null ? null : JsonConvert.DeserializeObject<T>((string) o);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tempData"></param>
        /// <param name="key"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T Peek<T>(this ITempDataDictionary tempData, string key) where T : class
        {
            var o = tempData.Peek(key);
            return o == null ? null : JsonConvert.DeserializeObject<T>((string) o);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tempData"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <typeparam name="T"></typeparam>
        public static void Put<T>(this ITempDataDictionary tempData, string key, T value) where T : class
        {
            tempData[key] = JsonConvert.SerializeObject(value);
        }
    }
}