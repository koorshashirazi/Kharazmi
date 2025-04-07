using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json.Serialization;
using Kharazmi.AspNetCore.Core.Extensions;
using Newtonsoft.Json;

namespace Kharazmi.AspNetCore.Core.Models;

public  record struct ExceptionMessage
{
    public ExceptionMessage()
    {
        Message = string.Empty;
        ExceptionType = string.Empty;
        Source = "Unknown Source";
        StackTrace = "No StackTrace available";
        Code = -1;
        InnerExceptionMessages = [];
    }

    [System.Text.Json.Serialization.JsonConstructor,  Newtonsoft.Json.JsonConstructor]
    public ExceptionMessage(string message, string exceptionType, string? source = null, string? stackTrace = null,
        int code = -1)
    {
        Message = message ?? throw new ArgumentNullException(nameof(message));
        ExceptionType = exceptionType ?? throw new ArgumentNullException(nameof(exceptionType));
        Source = source ?? "Unknown Source";
        StackTrace = stackTrace ?? "No StackTrace available";
        Code = code;
        InnerExceptionMessages = [];
    }

    public static ExceptionMessage FromException(Exception exception)
    {
        if (exception == null) throw new ArgumentNullException(nameof(exception));
        HashSet<InnerExceptionMessage> innerExceptionMessages = [];
        var innerException = exception.InnerException;

        while (innerException is not null)
        {
            var innerExceptionMessage = InnerExceptionMessage.FromException(innerException);
            innerExceptionMessages.Add(innerExceptionMessage);
            innerException = innerException.InnerException;
        }

        return new ExceptionMessage(
            message: exception.Message,
            exceptionType: exception.GetType().GetTypeFullName(),
            source: exception.Source,
            stackTrace: exception.StackTrace,
            code: exception.HResult
        )
        {
            InnerExceptionMessages = innerExceptionMessages.AsReadOnly()
        };
    }

    [JsonProperty, JsonInclude] public string Message { get; }

    [JsonProperty, JsonInclude] public string ExceptionType { get; }

    [JsonProperty, JsonInclude] public string Source { get; }

    [JsonProperty, JsonInclude] public string StackTrace { get; }

    [JsonProperty, JsonInclude] public int Code { get; }

    [JsonProperty, JsonInclude]
    public IReadOnlyCollection<InnerExceptionMessage> InnerExceptionMessages { get; private set; }


    public Exception? ToException()
    {
        try
        {
            Exception? currentException = null;
            foreach (var message in InnerExceptionMessages.Reverse())
            {
                var innerExceptionTypeValue = message.ExceptionType;

                if (string.IsNullOrWhiteSpace(innerExceptionTypeValue)) continue;

                Type? innerExceptionType = Type.GetType(innerExceptionTypeValue);

                if (innerExceptionType is null || !typeof(Exception).IsAssignableFrom(innerExceptionType))
                    return null;

                currentException = CreateExceptionInstance(innerExceptionType, message.Message, currentException);
                if (currentException is null) continue;

                SetExceptionProperties(currentException, message.Code, message.StackTrace,
                    message.Source);
            }

            var exceptionTypeValue = ExceptionType;
            if (string.IsNullOrWhiteSpace(exceptionTypeValue))
                return null;
            Type? exceptionType = Type.GetType(exceptionTypeValue);

            if (exceptionType == null || !typeof(Exception).IsAssignableFrom(exceptionType))
                return null;

            var exception = CreateExceptionInstance(exceptionType, Message, currentException);

            if (exception == null) return null;

            SetExceptionProperties(exception, Code, StackTrace, Source);

            return exception;
        }
        catch
        {
            return null;
        }
    }

    public TException? ToException<TException>() where TException : Exception, new()
    {
        return ToException() as TException;
    }

    private static Exception? CreateExceptionInstance(Type exceptionType, string message, Exception? currentException)
    {
        var constructorInfo = exceptionType.GetConstructor([typeof(string), typeof(Exception)]);

        Exception? exception = null;
        if (constructorInfo != null)
        {
            exception = (Exception?)constructorInfo.Invoke([message, currentException]);
        }

        if (exception == null)
        {
            var messageConstructor = exceptionType.GetConstructor([typeof(string)]);
            if (messageConstructor != null)
            {
                exception = (Exception?)messageConstructor.Invoke([message]);
            }
        }

        if (exception != null) return exception;

        var parameterlessConstructor = exceptionType.GetConstructor(Type.EmptyTypes);
        if (parameterlessConstructor != null)
        {
            exception = (Exception?)parameterlessConstructor.Invoke(null);
        }

        return exception;
    }

    private static void SetExceptionProperties<TException>(TException exception, int code, string stackTrace,
        string source)
        where TException : Exception
    {
        var exceptionType = typeof(Exception);

        // Set StackTrace
        var stackTraceField =
            exceptionType.GetField("_stackTraceString", BindingFlags.Instance | BindingFlags.NonPublic);
        stackTraceField?.SetValue(exception, stackTrace);

        // Set Source
        if (!string.IsNullOrEmpty(source))
        {
            exception.Source = source;
        }

        // Set HResult (Error Code)
        var hResultField = exceptionType.GetField("_HResult", BindingFlags.Instance | BindingFlags.NonPublic);
        hResultField?.SetValue(exception, code);
    }
}