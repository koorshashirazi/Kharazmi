using System;
using System.Text.Json.Serialization;
using Kharazmi.AspNetCore.Core.Extensions;
using Newtonsoft.Json;

namespace Kharazmi.AspNetCore.Core.Models;

public readonly record struct InnerExceptionMessage
{
    public InnerExceptionMessage()
    {
        Message = string.Empty;
        ExceptionType = string.Empty;
        Source = "Unknown Source";
        StackTrace = "No StackTrace available";
        Code = -1;
    }

    [System.Text.Json.Serialization.JsonConstructor,  Newtonsoft.Json.JsonConstructor]
    public InnerExceptionMessage(string message, string exceptionType, string? source = null, string? stackTrace = null,
        int code = -1)
    {
        Message = message ?? throw new ArgumentNullException(nameof(message));
        ExceptionType = exceptionType ?? throw new ArgumentNullException(nameof(exceptionType));
        Source = source ?? "Unknown Source";
        StackTrace = stackTrace ?? "No StackTrace available";
        Code = code;
    }

    public static InnerExceptionMessage FromException(Exception exception)
    {
        return new InnerExceptionMessage(
            message: exception.Message,
            exceptionType: exception.GetType().GetTypeFullName(),
            source: exception.Source,
            stackTrace: exception.StackTrace,
            code: exception.HResult
        );
    }


    [JsonProperty, JsonInclude] public string Message { get; }

    [JsonProperty, JsonInclude] public string ExceptionType { get; }

    [JsonProperty, JsonInclude] public string Source { get; }

    [JsonProperty, JsonInclude] public string StackTrace { get; }

    [JsonProperty, JsonInclude] public int Code { get; }
}