using System;
using System.Collections.Specialized;
using Newtonsoft.Json;

namespace Kharazmi.AspNetCore.Core.Serialization
{
    /// <summary>
    /// 
    /// </summary>
    public class NameValueCollectionConverter : JsonConverter
    {
        /// <summary>
        /// 
        /// </summary>
        public class NameValueCollectionItem
        {
            /// <summary>
            /// 
            /// </summary>
            public string Key { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string[] Values { get; set; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objectType"></param>
        /// <returns></returns>
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(NameValueCollection);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="objectType"></param>
        /// <param name="existingValue"></param>
        /// <param name="serializer"></param>
        /// <returns></returns>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var collection = existingValue as NameValueCollection;
            if (collection == null) collection = new NameValueCollection();

            var items = serializer.Deserialize<NameValueCollectionItem[]>(reader);
            if (items != null)
            {
                foreach (var item in items)
                {
                    if (item.Values != null)
                    {
                        foreach (var value in item.Values)
                        {
                            collection.Add(item.Key, value);
                        }
                    }
                }
            }

            return collection;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        /// <param name="serializer"></param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var collection = (NameValueCollection)value;
            if (collection != null)
            {
                var items = new NameValueCollectionItem[collection.AllKeys.Length];
                var index = 0;
                foreach (var key in collection.AllKeys)
                {
                    items[index++] = new NameValueCollectionItem
                    {
                        Key = key,
                        Values = collection.GetValues(key)
                    };
                }

                serializer.Serialize(writer, items);
            }
        }
    }
}