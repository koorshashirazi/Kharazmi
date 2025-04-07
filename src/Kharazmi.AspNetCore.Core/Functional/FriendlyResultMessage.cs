using System;
using System.Globalization;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace Kharazmi.AspNetCore.Core.Functional
{
    /// <summary>
    /// 
    /// </summary>
    public readonly record struct FriendlyResultMessage
    {
        public FriendlyResultMessage()
        {
            Description = string.Empty;
        }

        [System.Text.Json.Serialization.JsonConstructor,  Newtonsoft.Json.JsonConstructor]
        public FriendlyResultMessage(string description, int code = 0)
        {
            Description = description ?? throw new ArgumentNullException(nameof(description));
            Code = code;
        }

        public FriendlyResultMessage(ReadOnlySpan<char> description, int code = 0)
        {
            Description = new string(description.ToArray());
            Code = code;
        }

        [JsonProperty, JsonInclude] public string Description { get;  }
        [JsonProperty, JsonInclude] public string MessageType { get; } = nameof(FriendlyResultMessage);
        [JsonProperty, JsonInclude] public int Code { get;  }

        public static FriendlyResultMessage With(string description) => new(description);

        public static FriendlyResultMessage With(string description, int descriptionCode) =>
            new(description, descriptionCode);


        public override string ToString()
        {
#if DEBUG
            return $$"""
                     { 
                        "Description": "{{Description}}",
                        "Code": {{Code}}
                     }
                     """;
#else
        return $$"""
                 { "Description": "{{Description}}", "Code": {{Code}} }
                 """;
#endif
        }
    }
}