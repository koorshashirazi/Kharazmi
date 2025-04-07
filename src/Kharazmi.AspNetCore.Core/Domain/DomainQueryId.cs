using System;
using System.Text.Json.Serialization;
using Kharazmi.AspNetCore.Core.ValueObjects;
using Newtonsoft.Json;

namespace Kharazmi.AspNetCore.Core.Domain;

public readonly record struct DomainQueryId
{
    public DomainQueryId()
    {
        Value = Id.Default<string>();
    }

    [System.Text.Json.Serialization.JsonConstructor, Newtonsoft.Json.JsonConstructor]
    public DomainQueryId(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            throw new ArgumentNullException(nameof(value));
        }

        Value = value;
    }

    public static DomainQueryId New() => new(Id.New<string>());

    [JsonProperty, JsonInclude] public string Value { get; }

    public override string ToString()
    {
        return Value;
    }
    
    public static implicit operator string(DomainQueryId domainQueryId) => domainQueryId.ToString();
}