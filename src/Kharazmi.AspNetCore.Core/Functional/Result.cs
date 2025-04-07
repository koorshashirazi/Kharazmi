using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using Kharazmi.AspNetCore.Core.Extensions;
using Kharazmi.AspNetCore.Core.Validation;
using Newtonsoft.Json;

namespace Kharazmi.AspNetCore.Core.Functional
{
    internal static class ResultMessages
    {
        public static readonly string ErrorIsInaccessibleForSuccess =
            "You attempted to access the Error property for a successful result. A successful result has no Error.";

        public static readonly string ValueIsInaccessibleForFailure =
            "You attempted to access the Value property for a failed result. A failed result has no Value.";

        public static readonly string ErrorObjectIsNotProvidedForFailure =
            "You attempted to create a failure result, which must have an message, but a null message object was passed to the constructor.";

        public static readonly string ErrorObjectIsProvidedForSuccess =
            "You attempted to create a success result, which cannot have an message, but a non-null message object was passed to the constructor.";

        public static readonly string MessageModelIsNotProvidedForFailure =
            "You attempted to create a failure result, which must have an message, but a null or empty string was passed to the constructor.";

        public static readonly string MessageModelIsProvidedForSuccess =
            "You attempted to create a success result, which cannot have an message, but a non-null string was passed to the constructor.";

        public const string ExceptionMessage = "Can not complete its operations for some reason";
    }

    /// <summary>
    /// 
    /// </summary>
    public class Result
    {
        public const string NoDescription = "NO_DESCRIPTION";
        public const string SucceedDescription = "SUCCEED";
        public const string MaybeHasValueDescription = "MAYBE_HAS_VALUE";
        public const string FailedDescription = "FAILED";

        public static readonly FriendlyResultMessage NoDescriptionResultMessage = new(NoDescription);
        public static readonly FriendlyResultMessage FailedResultMessage = new(FailedDescription);
        public static readonly FriendlyResultMessage SucceedResultMessage = new(SucceedDescription);
        public static readonly FriendlyResultMessage MaybeResultMessage = new(MaybeHasValueDescription);

        private static readonly Result Success = new(false, SucceedResultMessage);
        
        [Newtonsoft.Json.JsonIgnore, System.Text.Json.Serialization.JsonIgnore]
        internal readonly Lazy<HashSet<InternalResultMessage>> InternalMessages = new(() => []);
        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="failed"></param>
        /// <param name="friendlyMessage"></param>
        protected internal Result(bool failed, FriendlyResultMessage friendlyMessage)
        {
            ResultId = Guid.NewGuid().ToString("N");
            CreateAt = DateTime.UtcNow.ToString("g", DateTimeFormatInfo.InvariantInfo);
            Failed = failed;
            ResultType = failed ? "Failed" : "Succeed";
            FriendlyMessage = friendlyMessage;
        }


        /// <summary>
        /// 
        /// </summary>
        [JsonProperty, JsonInclude]
        public bool Failed { get; }

        public string ResultType { get; }

        /// <summary></summary>
        [JsonProperty, JsonInclude]
        public string ResultId { get; protected set; }

        /// <summary></summary>
        [Newtonsoft.Json.JsonIgnore, System.Text.Json.Serialization.JsonIgnore]
        public string CreateAt { get; protected set; }

        [JsonProperty, JsonInclude] public FriendlyResultMessage FriendlyMessage { get; }

        /// <summary></summary>
        [JsonProperty, JsonInclude]
        public string RedirectToUrl { get; protected set; } = string.Empty;

        /// <summary></summary>
        [JsonProperty, JsonInclude]
        public string JsHandler { get; protected set; } = string.Empty;

        /// <summary></summary>
        [JsonProperty, JsonInclude]
        public string RequestPath { get; protected set; } = string.Empty;

        [Newtonsoft.Json.JsonIgnore, System.Text.Json.Serialization.JsonIgnore]
        public string TraceId { get; protected set; } = string.Empty;

        /// <summary></summary>
        [JsonProperty, JsonInclude]
        public int ResponseStatus { get; protected set; }

        [Newtonsoft.Json.JsonIgnore, System.Text.Json.Serialization.JsonIgnore]
        public Exception? Exception { get; protected set; }

        public HashSet<FriendlyResultMessage> Messages { get; } = [];

        public HashSet<ValidationFailure> ValidationMessages { get; } = [];

        
        public IReadOnlyCollection<InternalResultMessage> GetInternalMessages() => InternalMessages.Value.AsReadOnly();
        
        public Result WithInternalMessages(params IReadOnlyCollection<InternalResultMessage> messages)
        {
            if (messages == null) throw new ArgumentNullException(nameof(messages));
            foreach (var message in messages)
            {
                InternalMessages.Value.Add(message);
            }

            return this;
        }
        
        public Result WithException(Exception? exception)
        {
            if (exception != null)
            {
                Exception = Exception is null ? exception : new AggregateException(Exception, exception);
            }

            return this;
        }

        [DebuggerStepThrough]
        public Result WithMessages(params IReadOnlyCollection<FriendlyResultMessage> messages)
        {
            if (messages is null)
            {
                throw new ArgumentNullException(nameof(messages));
            }

            foreach (var message in messages)
            {
                Messages.Add(message);
            }

            return this;
        }

        [DebuggerStepThrough]
        public Result WithValidationMessages(params IReadOnlyCollection<ValidationFailure> failures)
        {
            if (failures is null)
            {
                throw new ArgumentNullException(nameof(failures));
            }

            foreach (var message in failures)
            {
                ValidationMessages.Add(message);
            }

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public Result WithRedirectUrl(string value)
        {
            RedirectToUrl = value;
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public Result WithJsHandler(string value)
        {
            JsHandler = value;
            return this;
        }


        public Result WithRequestPath(string value)
        {
            RequestPath = value;
            return this;
        }

        public Result WithTraceId(string value)
        {
            TraceId = value;
            return this;
        }

        public Result WithStatus(int value)
        {
            ResponseStatus = value;
            return this;
        }


        [DebuggerStepThrough]
        public static Result Ok() => Success;

        public static Result Ok(FriendlyResultMessage message) => new(false, message);

        [DebuggerStepThrough]
        public static Result<T> Ok<T>(T value) => new(value, false, SucceedResultMessage);

        public static Result<T> Ok<T>(T value, FriendlyResultMessage message) => new(value, false, message);

        [DebuggerStepThrough]
        public static Result Fail(string description, int code = 0)
            => new(true, new FriendlyResultMessage(description, code));

        [DebuggerStepThrough]
        public static Result Fail(FriendlyResultMessage? friendlyMessage)
            => new(true, friendlyMessage ?? FailedResultMessage);


        [DebuggerStepThrough]
        public static Result<T> Fail<T>(string description, int code = 0)
            => new(default, true, new FriendlyResultMessage(description, code));


        [DebuggerStepThrough]
        public static Result<T> Fail<T>(FriendlyResultMessage friendlyMessage)
            => new(default, true, friendlyMessage);


        public static Result<T> MapToFail<T>(Result result)
        {
            if (result is null) throw new ArgumentNullException(nameof(result));

            return Fail<T>(result.FriendlyMessage)
                .WithMessages([.. result.Messages])
                .WithValidationMessages([.. result.ValidationMessages])
                .WithTraceId(result.TraceId)
                .WithStatus(result.ResponseStatus)
                .WithJsHandler(result.JsHandler)
                .WithRedirectUrl(result.RedirectToUrl)
                .WithRequestPath(result.RequestPath)
                .WithException(result.Exception);
        }

        public static Result<T> MapToOk<T>(T value, Result result)
        {
            if (value is null) throw new ArgumentNullException(nameof(value));
            if (result is null) throw new ArgumentNullException(nameof(result));

            return Ok(value, result.FriendlyMessage)
                .WithMessages([.. result.Messages])
                .WithValidationMessages([.. result.ValidationMessages])
                .WithTraceId(result.TraceId)
                .WithStatus(result.ResponseStatus)
                .WithJsHandler(result.JsHandler)
                .WithRedirectUrl(result.RedirectToUrl)
                .WithRequestPath(result.RequestPath);
        }

        [DebuggerStepThrough]
        public static Result Combine(params Result[] results)
        {
            if (results == null) throw new ArgumentNullException(nameof(results));
            var resultList = results.Where(x => !x.Failed).ToList();

            if (resultList.Count == 0) return Ok();

            var message = resultList[0].FriendlyMessage;
            var errors = resultList.SelectMany(r => r.Messages).ToArray();
            var failures = resultList.SelectMany(r => r.ValidationMessages).ToArray();
            return Fail(message)
                .WithMessages([.. resultList.Select(x => x.FriendlyMessage).Skip(1)])
                .WithMessages(errors)
                .WithValidationMessages(failures);
        }


        [DebuggerStepThrough]
        public static Result Combine<T>(params Result<T>[] results)
        {
            var untyped = results.Select(Result (result) => result).ToArray();
            return Combine(untyped);
        }

        public override string ToString()
        {
            StringBuilder jsonBuilder = new("{");

            jsonBuilder.AppendInvariant($"\"Failed\": {Failed.ToString().ToLowerInvariant()},");
            jsonBuilder.AppendInvariant($"\"ResultType\": \"{ResultType}\",");
            jsonBuilder.AppendInvariant($"\"ResultId\": \"{ResultId}\",");
            jsonBuilder.AppendInvariant($"\"ResponseStatus\": {ResponseStatus},");

            if (TraceId.IsNotEmpty())
            {
                jsonBuilder.AppendInvariant($"\"TraceId\":\"{TraceId}\",");
            }

            if (RequestPath.IsNotEmpty())
            {
                jsonBuilder.AppendInvariant($"\"RequestPath\":\"{RequestPath}\",");
            }

            if (RedirectToUrl.IsNotEmpty())
            {
                jsonBuilder.AppendInvariant($"\"RedirectToUrl\":\"{RedirectToUrl}\",");
            }

            if (JsHandler.IsNotEmpty())
            {
                jsonBuilder.AppendInvariant($"\"JsHandler\":\"{JsHandler}\",");
            }


            if (Messages.Count > 0)
            {
                var messages = Messages.GroupBy(x => x.MessageType).ToArray().AsSpan();
                foreach (var groupedMessage in messages)
                {
                    var typeName = groupedMessage.Key;
                    jsonBuilder.AppendInvariant($"\"{typeName}s\":[");

                    foreach (var resultMessage in groupedMessage)
                    {
                        jsonBuilder.Append(resultMessage);
                        jsonBuilder.Append(',');
                    }

                    RemoveLastChar();

                    jsonBuilder.Append(']');
                    jsonBuilder.Append(',');
                }
            }

            if (ValidationMessages.Count > 0)
            {
                jsonBuilder.Append("\"ValidationMessages\":[");
                foreach (var result in ValidationMessages)
                {
                    jsonBuilder.Append(result);
                    jsonBuilder.Append(',');
                }

                RemoveLastChar();

                jsonBuilder.Append(']');
                jsonBuilder.Append(',');
            }
            
            if (InternalMessages.Value.Count != 0)
            {
                jsonBuilder.Append("\"InternalMessages\":[");
                foreach (var result in InternalMessages.Value)
                {
                    jsonBuilder.Append(result.ToString());
                    jsonBuilder.Append(',');
                }

                RemoveLastChar();

                jsonBuilder.Append(']');
                jsonBuilder.Append(',');
            }

            if (Exception != null)
            {
                jsonBuilder.AppendInvariant($"\"Exceptions\": {Exception.AsJsonException()}");
            }

            jsonBuilder.Append('}');
            
            return jsonBuilder.ToString();

            void RemoveLastChar()
            {
                var lastIndex = jsonBuilder.ToString().LastIndexOf(',');
                if (lastIndex > 0)
                {
                    jsonBuilder.Remove(jsonBuilder.Length - 1, 1);
                }
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Result<T> : Result
    {
        private readonly T? _value;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="failed"></param>
        /// <param name="message"></param>
        protected internal Result(T? value, bool failed, FriendlyResultMessage message) : base(failed, message)
        {
            _value = value;
        }

        public bool HasValue() => !Failed && _value is not null;

        /// <summary>
        /// 
        /// </summary>
        public T Value => HasValue() ? _value! : throw new InvalidOperationException("There is no value for failure.");


        public new Result<T> WithException(Exception? exception)
        {
            base.WithException(exception);

            return this;
        }
        public new  Result<T> WithInternalMessages(params IReadOnlyCollection<InternalResultMessage> messages)
        {
            base.WithInternalMessages(messages);
            return this;
        }

        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="messages"></param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public new Result<T> WithMessages(params IReadOnlyCollection<FriendlyResultMessage> messages)
        {
            base.WithMessages(messages);

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="failures"></param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public new Result<T> WithValidationMessages(params IReadOnlyCollection<ValidationFailure> failures)
        {
            base.WithValidationMessages(failures);
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public new Result<T> WithRedirectUrl(string value)
        {
            RedirectToUrl = value;
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public new Result<T> WithJsHandler(string value)
        {
            JsHandler = value;
            return this;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public new Result<T> WithRequestPath(string value)
        {
            RequestPath = value;
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public new Result<T> WithTraceId(string value)
        {
            TraceId = value;
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public new Result<T> WithStatus(int value)
        {
            ResponseStatus = value;
            return this;
        }
    }
}