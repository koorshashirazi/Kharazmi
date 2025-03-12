using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Kharazmi.AspNetCore.Core.Exceptions;

namespace Kharazmi.AspNetCore.Core.Domain.Aggregates
{
    public class MetadataContainer : ConcurrentDictionary<string, string>
    {
        public MetadataContainer()
        {
        }

        public MetadataContainer(IDictionary<string, string> keyValuePairs)
            : base(keyValuePairs)
        {
        }

        public MetadataContainer(IEnumerable<KeyValuePair<string, string>> keyValuePairs)
            : base(keyValuePairs.ToDictionary(kv => kv.Key, kv => kv.Value))
        {
        }

        public MetadataContainer(params KeyValuePair<string, string>[] keyValuePairs)
            : this((IEnumerable<KeyValuePair<string, string>>)keyValuePairs)
        {
        }

        public void AddRange(params KeyValuePair<string, string>[] keyValuePairs)
        {
            AddRange((IEnumerable<KeyValuePair<string, string>>)keyValuePairs);
        }

        public void AddRange(IEnumerable<KeyValuePair<string, string>> keyValuePairs)
        {
            if (keyValuePairs is null)
            {
                throw new ArgumentNullException(nameof(keyValuePairs));
            }

            foreach (var keyValuePair in keyValuePairs)
            {
                TryAdd(keyValuePair.Key, keyValuePair.Value);
            }
        }

        public override string ToString()
        {
            return string.Join(Environment.NewLine, this.Select(kv => $"{kv.Key}: {kv.Value}"));
        }

        public string GetMetadataValue(string key)
        {
            return GetMetadataValue(key, s => s);
        }

        public T GetMetadataValue<T>(string key, Func<string, T> converter)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException($"'{nameof(key)}' cannot be null or empty.", nameof(key));
            }

            if (converter is null)
            {
                throw new ArgumentNullException(nameof(converter));
            }

            if (!TryGetValue(key, out var value))
            {
                throw new MetaDataNotFindKeyException($"Could not find metadata key '{key}'");
            }

            try
            {
                return converter(value);
            }
            catch (Exception e)
            {
                throw new MetaDataException($"Failed to parse metadata key '{key}' with value '{value}' due to '{e.Message}'", e);
            }
        }
    }
}