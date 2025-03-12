using System;

namespace Kharazmi.AspNetCore.Core.Functional;

public readonly record struct InternalResultMessage
{
    public InternalResultMessage()
    {
        Category = string.Empty;
        Message = string.Empty;
    }

    public InternalResultMessage(string category, string message)
    {
        Category = category ?? throw new ArgumentNullException(nameof(category));
        Message = message ?? throw new ArgumentNullException(nameof(message));
    }


    [System.Text.Json.Serialization.JsonIgnore, Newtonsoft.Json.JsonIgnore]
    public string Category { get; }

    [System.Text.Json.Serialization.JsonIgnore, Newtonsoft.Json.JsonIgnore]
    public string Message { get; }


    public override string ToString()
    {
#if DEBUG
        return $$"""
                 { 
                    "Category": "{{Category}}",
                    "Message": "{{Message}}"
                 }
                 """;
#else
        return $$"""
                 { "Category": "{{Category}}", "Message": "{{Message}}" }
                 """;
#endif
    }

    public static InternalResultMessage With(string category, string message) =>
        new(category, message);


    public static InternalResultMessage With(string category, params string[] messages)
    {
        var message = messages.Length == 1 ? messages[0] : string.Join(",", messages);
        return new InternalResultMessage(category, message);
    }
}