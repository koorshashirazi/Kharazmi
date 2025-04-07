using System;
using Kharazmi.AspNetCore.Core.Domain;
using Kharazmi.AspNetCore.Core.GuardToolkit;

namespace Kharazmi.AspNetCore.Core.EventSourcing
{
    public interface IEventSerializer
    {
        IDomainEvent? Deserialize(string serializedEvent, Type eventType);
        TEvent? Deserialize<TEvent>(string serializedEvent) where TEvent : IDomainEvent;

        string Serialize<TEvent>(TEvent domainEvent) where TEvent : IDomainEvent;
    }


    public class JsonEventSerializer(IJsonSerializer jsonSerializer) : IEventSerializer
    {
        private readonly IJsonSerializer _jsonSerializer =
            Ensure.ArgumentIsNotNull(jsonSerializer, nameof(jsonSerializer));

        public IDomainEvent? Deserialize(string serializedEvent, Type eventType)
        {
            return _jsonSerializer.Deserialize(serializedEvent, eventType) as IDomainEvent;
        }

        public TEvent? Deserialize<TEvent>(string serializedEvent) where TEvent : IDomainEvent
        {
            return _jsonSerializer.Deserialize<TEvent>(serializedEvent);
        }

        public string Serialize<TEvent>(TEvent domainEvent) where TEvent : IDomainEvent
        {
            return _jsonSerializer.Serialize<TEvent>(domainEvent);
        }
    }
}