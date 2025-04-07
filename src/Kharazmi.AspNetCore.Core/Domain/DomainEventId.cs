using System;
using System.Text.Json.Serialization;
using Kharazmi.AspNetCore.Core.ValueObjects;
using Newtonsoft.Json;

namespace Kharazmi.AspNetCore.Core.Domain;

public readonly record struct DomainEventId
{
    public DomainEventId()
    {
        Value = Id.Default<string>();
    }

    [System.Text.Json.Serialization.JsonConstructor, Newtonsoft.Json.JsonConstructor]
    public DomainEventId(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            throw new ArgumentNullException(nameof(value));
        }

        Value = value;
    }

    public static DomainEventId New() => new(Id.New<string>());

    [JsonProperty, JsonInclude] public string Value { get; }

    public override string ToString()
    {
        return Value;
    }
    
    public static implicit operator string(DomainEventId domainEventId) => domainEventId.ToString();
}