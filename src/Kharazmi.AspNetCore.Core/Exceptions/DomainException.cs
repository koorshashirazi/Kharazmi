using System;
using System.Collections.Generic;
using System.Linq;
using Kharazmi.AspNetCore.Core.Extensions;
using Kharazmi.AspNetCore.Core.Functional;
using Kharazmi.AspNetCore.Core.Validation;

namespace Kharazmi.AspNetCore.Core.Exceptions
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class DomainException : FrameworkException
    {
        /// <summary></summary>
        public string AggregateId { get; protected set; }  
        /// <summary></summary>
        public string MessageType { get; protected set; }
        public IReadOnlyCollection<MessageModel?> ErrorMessages { get; protected set; }
        public IReadOnlyCollection<ValidationFailure> ValidationFailures { get; protected set; }
        public IReadOnlyCollection<MessageModel> ExceptionErrors { get; protected set; }

        private DomainException(string message, Exception exception, string description, string code,
            IEnumerable<MessageModel?> messageErrors,
            IEnumerable<ValidationFailure> validationFailures,
            IEnumerable<MessageModel> exceptionErrors) : base(message, exception, description, code)
        {
            ErrorMessages = messageErrors.AsReadOnly();
            ValidationFailures = validationFailures.AsReadOnly();
            ExceptionErrors = exceptionErrors.AsReadOnly();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static DomainException Empty() =>
            new DomainException("", null, "", "", Enumerable.Empty<MessageModel>(),
                Enumerable.Empty<ValidationFailure>(), Enumerable.Empty<MessageModel>());


        public static DomainException For(string message, Exception exception = null) =>
            new DomainException(message, exception, "", "", Enumerable.Empty<MessageModel>(),
                Enumerable.Empty<ValidationFailure>(), Enumerable.Empty<MessageModel>());

        public DomainException WithAggregateId(string value)
        {
            AggregateId = value;
            return this;
        }
        
        public DomainException WithMessageType(string value)
        {
            MessageType = value;
            return this;
        }
  
        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        /// <returns></returns>
        public static DomainException From(Result result, string message = "", Exception exception = null)
        {
            return For(message, exception)
                .WithMessageType(result?.MessageType)
                .AddErrorMessages(result?.Messages)
                .AddValidationMessages(result?.ValidationMessages)
                .WithCode(result?.Code)
                .WithDescription(result?.Description)
                .ToDomainException();
        }

        public static DomainException From(MessageModel messageModel, string message = "", Exception exception = null)
        {
            return For(message, exception)
                .WithMessageType(messageModel?.MessageType)
                .WithCode(messageModel?.Code)
                .WithDescription(messageModel?.Description)
                .ToDomainException();
        }

        public DomainException AddErrorMessage(MessageModel? error)
        {
            if (ErrorMessages == null)
                ErrorMessages = new List<MessageModel?>();

            if (error != null)
            {
                var exceptionsErrors = ErrorMessages.ToList();
                exceptionsErrors.Add(error);
                ErrorMessages = exceptionsErrors.AsReadOnly();
            }

            return this;
        }

        public DomainException AddErrorMessages(IEnumerable<MessageModel?> errors)
        {
            if (ErrorMessages == null)
                ErrorMessages = new List<MessageModel?>();

            if (errors != null)
            {
                var exceptionsErrors = ErrorMessages.ToList();
                exceptionsErrors.AddRange(errors);
                ErrorMessages = exceptionsErrors.AsReadOnly();
            }

            return this;
        }

        public DomainException AddValidationMessage(ValidationFailure validationFailure)
        {
            if (ValidationFailures == null)
                ValidationFailures = new List<ValidationFailure>();

            if (validationFailure != null)
            {
                var messages = ValidationFailures.ToList();
                messages.Add(validationFailure);
                ValidationFailures = messages.AsReadOnly();
            }

            return this;
        }

        public DomainException AddValidationMessages(IEnumerable<ValidationFailure> validationFailures)
        {
            if (ValidationFailures == null)
                ValidationFailures = new List<ValidationFailure>();

            if (validationFailures != null)
            {
                var messages = ValidationFailures.ToList();
                messages.AddRange(validationFailures);
                ValidationFailures = messages.AsReadOnly();
            }

            return this;
        }

        public DomainException AddExceptionMessage(MessageModel error)
        {
            if (ExceptionErrors == null)
                ExceptionErrors = new List<MessageModel>();

            if (error != null)
            {
                var messages = ExceptionErrors.ToList();
                messages.Add(error);
                ExceptionErrors = messages.AsReadOnly();
            }

            return this;
        }

        public DomainException AddExceptionMessages(IEnumerable<MessageModel> errors)
        {
            if (ExceptionErrors == null)
                ExceptionErrors = new List<MessageModel>();

            if (errors != null)
            {
                var messages = ExceptionErrors.ToList();
                messages.AddRange(errors);
                ExceptionErrors = messages.AsReadOnly();
            }

            return this;
        }
    }
}