﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Web;
using Kharazmi.AspNetCore.Core.GuardToolkit;
using Microsoft.Extensions.Primitives;

namespace Kharazmi.AspNetCore.Core.Extensions
{
    public static partial class Core
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public static NameValueCollection AsNameValueCollection(this IDictionary<string, StringValues> collection)
        {
            var nv = new NameValueCollection();

            foreach (var field in collection)
            {
                nv.Add(field.Key, field.Value.First());
            }

            return nv;
        }
        
        public static void AddRange<T, U>(this IDictionary<T, U> values, IEnumerable<KeyValuePair<T, U>> other)
        {
            foreach (var kvp in other)
            {
                if (values.ContainsKey(kvp.Key))
                    throw new ArgumentException("An item with the same key has already been added.");
                values.Add(kvp);
            }
        }

        public static void Merge(this IDictionary<string, object> instance, string key, object value,
            bool replaceExisting = true)
        {
            if (replaceExisting || !instance.ContainsKey(key))
                instance[key] = value;
        }

     
        public static void Merge<T, U>(this IDictionary<T, U> instance, IDictionary<T, U> from,
            bool replaceExisting = true)
        {
            foreach (var keyValuePair in from)
                if (replaceExisting || !instance.ContainsKey(keyValuePair.Key))
                    instance[keyValuePair.Key] = keyValuePair.Value;
        }

        public static void AppendInValue(this IDictionary<string, object> instance, string key, string separator,
            object value)
        {
            instance[key] = !instance.ContainsKey(key) ? value.ToString() : instance[key] + separator + value;
        }

        public static void PrependInValue(this IDictionary<string, object> instance, string key, string separator,
            object value)
        {
            instance[key] = !instance.ContainsKey(key) ? value.ToString() : value + separator + instance[key];
        }

        public static string ToAttributeString(this IDictionary<string, object> instance)
        {
            var builder = new StringBuilder();
            foreach (var pair in instance)
            {
                var args = new object[]
                    {HttpUtility.HtmlAttributeEncode(pair.Key), HttpUtility.HtmlAttributeEncode(pair.Value.ToString())};
                builder.Append(" {0}=\"{1}\"".FormatWith(args));
            }
            return builder.ToString();
        }


        public static ExpandoObject ToExpandoObject(this IDictionary<string, object> source,
            bool castIfPossible = false)
        {
            Guard.ArgumentNotNull(source, nameof(source));

            if (castIfPossible && source is ExpandoObject)
                return (ExpandoObject) source;

            var result = new ExpandoObject();
            result.AddRange(source);

            return result;
        }
        
        /// <summary>
        /// Gets a value from the dictionary with given key. Returns default value if can not find.
        /// </summary>
        /// <param name="dictionary">Dictionary to check and get</param>
        /// <param name="key">Key to find the value</param>
        /// <typeparam name="TKey">Type of the key</typeparam>
        /// <typeparam name="TValue">Type of the value</typeparam>
        /// <returns>Value if found, default if can not found.</returns>
        public static TValue GetOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
        {
            return dictionary.TryGetValue(key, out var obj) ? obj : default(TValue);
        }

        public static IReadOnlyDictionary<TKey, TValue> AsReadOnly<TKey, TValue>(
            this IDictionary<TKey, TValue> dictionary)
        {
            return new ReadOnlyDictionary<TKey, TValue>(dictionary);
        }
    }
}