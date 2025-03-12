using System;
using Kharazmi.AspNetCore.Core.Helpers;
using Newtonsoft.Json;

namespace Kharazmi.AspNetCore.Core.EventSourcing
{
    public interface IJsonSerializer
    {
        object? Deserialize(string serializedEvent, Type eventType);
        T? Deserialize<T>(string serializedEvent);

        string Serialize<T>(T domainEvent);
    }

    public class JsonSerializer : IJsonSerializer
    {
        public object? Deserialize(string serializedEvent, Type eventType)
        {
            return JsonConvert.DeserializeObject(serializedEvent, eventType, JsonSerializerHelper.DefaultJsonSettings);
        }

        public T? Deserialize<T>(string serializedEvent)
        {
            return JsonConvert.DeserializeObject<T>(serializedEvent, JsonSerializerHelper.DefaultJsonSettings);
        }

        public string Serialize<T>(T domainEvent)
        {
            return JsonConvert.SerializeObject(domainEvent, JsonSerializerHelper.DefaultJsonSettings);
        }
    }
}