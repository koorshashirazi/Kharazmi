using System;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace Kharazmi.AspNetCore.Core.Functional;

public readonly record struct FriendlyResultMessage
{
    public FriendlyResultMessage()
    {
        Description = string.Empty;
    }

    [System.Text.Json.Serialization.JsonConstructor, Newtonsoft.Json.JsonConstructor]
    public FriendlyResultMessage(string description, int descriptionCode = 0)
    {
        Description = description ?? throw new ArgumentNullException(nameof(description));
        DescriptionCode = descriptionCode;
    }


    [JsonProperty, JsonInclude] public string Description { get; }
    [JsonProperty, JsonInclude] public string MessageType { get; } = nameof(FriendlyResultMessage);
    [JsonProperty, JsonInclude] public int DescriptionCode { get; }

    public static FriendlyResultMessage With(string description) => new(description);

    public static FriendlyResultMessage With(string description, int descriptionCode) =>
        new(description, descriptionCode);


    public override string ToString()
    {
#if DEBUG
        return $$"""
                 { 
                    "Description": "{{Description}}",
                    "DescriptionCode": "{{DescriptionCode}}"
                 }
                 """;
#else
        return $$"""
                 { "Description": "{{Description}}", "DescriptionCode": "{{DescriptionCode}}" }
                 """;
#endif
    }
}