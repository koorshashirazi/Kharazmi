using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json.Serialization;
using Kharazmi.AspNetCore.Core.Extensions;
using Kharazmi.AspNetCore.Core.Validation;

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
        private static readonly Result _ok = new Result(false, "", "",
            Enumerable.Empty<MessageModel>(), Enumerable.Empty<ValidationFailure>());

        /// <summary>
        /// 
        /// </summary>
        /// <param name="failed"></param>
        /// <param name="code"></param>
        /// <param name="messages"></param>
        /// <param name="failures"></param>
        /// <param name="description"></param>
        protected Result(
            bool failed,
            string description,
            string code,
            IEnumerable<MessageModel?> messages,
            IEnumerable<ValidationFailure> failures)
        {
            ResultId = Guid.NewGuid().ToString("N");
            CreateAt = DateTime.Now.ToString("g");

            var validationFailures = failures.AsReadOnly();
            var fail = validationFailures.Any() || failed;

            Failed = fail;
            ResultType = fail ? "Error" : "Success";
            Description = description;
            Code = code;
            Messages = messages.AsReadOnly();
            ValidationMessages = validationFailures;
        }

        /// <summary>
        /// 
        /// </summary>
        public bool Failed { get; }

        /// <summary></summary>
        public string ResultId { get; protected set; }

        /// <summary></summary>
        public string CreateAt { get; protected set; }
        
        /// <summary></summary>
        public string MessageType { get; protected set; }

        /// <summary></summary>
        public string Code { get; protected set; }

        /// <summary></summary>
        public string Description { get; protected set; }

        /// <summary></summary>
        public string ResultType { get; protected set; }

        /// <summary></summary>
        public string RedirectToUrl { get; protected set; }

        /// <summary></summary>
        public string JsHandler { get; protected set; }

        /// <summary></summary>
        public string RequestPath { get; protected set; }

        [Newtonsoft.Json.JsonIgnore, JsonIgnore]
        public string TraceId { get; protected set; }

        /// <summary></summary>
        public int Status { get; protected set; }
        
        [Newtonsoft.Json.JsonIgnore, JsonIgnore]
        public Exception? Exception { get; protected set; }

        public IReadOnlyList<MessageModel?> Messages { get; protected set; }

        public IReadOnlyList<ValidationFailure> ValidationMessages { get; protected set; }

        public Result AddException(Exception? exception)
        {
            if (exception != null)
            {
                Exception = Exception is null ? exception : new AggregateException(Exception, exception);
            }

            return this;
        }
        
        [DebuggerStepThrough]
        public Result WithMessages(IEnumerable<MessageModel?> messages)
        {
            Messages = messages.AsReadOnly();
            return this;
        }

        [DebuggerStepThrough]
        public Result WithValidationMessages(IEnumerable<ValidationFailure> failures)
        {
            ValidationMessages = failures.AsReadOnly();
            return this;
        }

        [DebuggerStepThrough]
        public Result AddMessage(string description, string code = "")
        {
            var errors = Messages.ToList();
            errors.Add(MessageModel.For(description, code));
            Messages = errors;
            return this;
        }

        [DebuggerStepThrough]
        public Result AddMessage(MessageModel? message)
        {
            var messages = Messages.ToList();
            if (message != null)
                messages.Add(message);

            Messages = messages;
            return this;
        }

        [DebuggerStepThrough]
        public Result AddValidationMessage(ValidationFailure failure)
        {
            var failures = ValidationMessages.ToList();
            if (failure != null)
                failures.Add(failure);

            ValidationMessages = failures;
            return this;
        }

        public Result WithMessageType(string value)
        {
            MessageType = value;
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
            Status = value;
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public Result UpdateResultType(string value)
        {
            ResultType = value;
            return this;
        }

        [DebuggerStepThrough]
        public static Result Ok() => _ok;

        public static Result Ok(string description, string code = "")
            => new Result(false, description, code,
                Enumerable.Empty<MessageModel>(), Enumerable.Empty<ValidationFailure>());

        public static Result Ok(MessageModel messageModel)
            => new Result(false, messageModel.Description, messageModel.Code,
                Enumerable.Empty<MessageModel>(), Enumerable.Empty<ValidationFailure>());

        [DebuggerStepThrough]
        public static Result<T> Ok<T>(T value) => new Result<T>(value, false, "", "");

        public static Result<T> Ok<T>(T value, string description, string code = "") =>
            new Result<T>(value, false, description, code);

        public static Result<T> Ok<T>(T value, MessageModel messageModel) =>
            new Result<T>(value, false, messageModel?.Description, messageModel?.Code);

        public static Result<T> Empty<T>(string description= "", string code = "") =>
            new Result<T>(Enumerable.Empty<T>().FirstOrDefault(), false, description, code);
        
        public static Result<T> Empty<T>(MessageModel messageModel = null) =>
            new Result<T>(Enumerable.Empty<T>().FirstOrDefault(), false, messageModel?.Description, messageModel.Code);

        [DebuggerStepThrough]
        public static Result Fail(string description, string code = "")
            => new Result(true, description, code, Enumerable.Empty<MessageModel>(),
                Enumerable.Empty<ValidationFailure>());

        [DebuggerStepThrough]
        public static Result Fail(MessageModel? messageModel)
            => Fail(messageModel?.Description, messageModel?.Code);

        [DebuggerStepThrough]
        public static Result<T> Fail<T>(string description, string code = "")
            => new Result<T>(default, true, description, code);


        [DebuggerStepThrough]
        public static Result<T> Fail<T>(MessageModel messageModel)
            => Fail<T>(messageModel?.Description, messageModel?.Code);


        public static Result<T> MapToFail<T>(Result result)
        {
            if (result == null)
                return Fail<T>("Failed  mapping");
            
            return Fail<T>(result.Description, result.Code)
                .WithMessages(result.Messages)
                .WithValidationMessages(result.ValidationMessages)
                .WithTraceId(result.TraceId)
                .WithStatus(result.Status)
                .WithJsHandler(result.JsHandler)
                .WithRedirectUrl(result.RedirectToUrl)
                .WithRequestPath(result.RequestPath)
                .UpdateResultType(result.ResultType);
        }

        public static Result<T> MapToOk<T>(T value, Result result)
        {
            if (result == null || value == null)
                return Fail<T>("Failed  mapping");
            
            return Ok(value, result.Description, result.Code)
                .WithMessages(result.Messages)
                .WithValidationMessages(result.ValidationMessages)
                .WithTraceId(result.TraceId)
                .WithStatus(result.Status)
                .WithJsHandler(result.JsHandler)
                .WithRedirectUrl(result.RedirectToUrl)
                .WithRequestPath(result.RequestPath)
                .UpdateResultType(result.ResultType);
        }

        [DebuggerStepThrough]
        public static Result Combine(string seperator, params Result[] results)
        {
            var resultList = results.Where(x => !x.Failed).ToList();

            if (!resultList.Any()) return Ok();

            var message = string.Join(seperator, resultList.Select(x => x.Description).ToArray());
            var code = string.Join(seperator, resultList.Select(x => x.Code).ToArray());
            var messageType = string.Join(seperator, resultList.Select(x => x.MessageType).ToArray());
            var errors = resultList.SelectMany(r => r.Messages).ToList();
            var failures = resultList.SelectMany(r => r.ValidationMessages).ToList();

            return Fail(message, code)
                .WithMessageType(messageType)
                .WithMessages(errors)
                .WithValidationMessages(failures);
        }

        [DebuggerStepThrough]
        public static Result Combine(params Result[] results) => Combine(", ", results);

        [DebuggerStepThrough]
        public static Result Combine<T>(params Result<T>[] results) => Combine(", ", results);

        [DebuggerStepThrough]
        public static Result Combine<T>(string seperator, params Result<T>[] results)
        {
            var untyped = results.Select(result => (Result) result).ToArray();
            return Combine(seperator, untyped);
        }

        public override string ToString()
        {
            return !Failed
                ? "Ok"
                : $"Failed with NotificationId {Code} , Description {Description} ";
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Result<T> : Result
    {
        private readonly T _value;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="failed"></param>
        /// <param name="description"></param>
        /// <param name="code"></param>
        protected internal Result(T value, bool failed, string description, string code)
            : base(failed, description, code, Enumerable.Empty<MessageModel>(), Enumerable.Empty<ValidationFailure>())
        {
            _value = value;
        }


        /// <summary>
        /// 
        /// </summary>
        public T Value => !Failed ? _value : throw new InvalidOperationException("There is no value for failure.");


        public new Result<T> AddException(Exception? exception)
        {
            if (exception != null)
            {
                Exception = Exception is null ? exception : new AggregateException(Exception, exception);
            }

            return this;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="messages"></param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public new Result<T> WithMessages(IEnumerable<MessageModel?> messages)
        {
            Messages = messages.AsReadOnly();
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="failures"></param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public new Result<T> WithValidationMessages(IEnumerable<ValidationFailure> failures)
        {
            ValidationMessages = failures.AsReadOnly();
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="description"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public new Result<T> AddMessage(string description, string code = "")
        {
            var messages = Messages.ToList();
            messages.Add(MessageModel.For(description, code));
            Messages = messages;
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="error"></param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public new Result<T> AddMessage(MessageModel? error)
        {
            var errors = Messages.ToList();
            if (error != null)
                errors.Add(error);

            Messages = errors;
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="failure"></param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public new Result<T> AddValidationMessage(ValidationFailure failure)
        {
            var failures = ValidationMessages.ToList();
            if (failure != null)
                failures.Add(failure);

            ValidationMessages = failures;
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
            Status = value;
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public new Result<T> UpdateResultType(string value)
        {
            ResultType = value;
            return this;
        }
    }
}