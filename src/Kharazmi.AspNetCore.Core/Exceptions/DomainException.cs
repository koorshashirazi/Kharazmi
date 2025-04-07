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

        public HashSet<FriendlyResultMessage> ErrorMessages { get; protected set; }
        public HashSet<ValidationFailure> ValidationFailures { get; protected set; }
        public HashSet<InternalResultMessage> ExceptionErrors { get; protected set; }

        private DomainException(string message, Exception? exception, string description, string code,
            HashSet<FriendlyResultMessage> messageErrors,
            HashSet<ValidationFailure> validationFailures,
            HashSet<InternalResultMessage> exceptionErrors) : base(message, exception, description, code)
        {
            ErrorMessages = messageErrors;
            ValidationFailures = validationFailures;
            ExceptionErrors = exceptionErrors;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static DomainException Empty() =>
            new("", null, "", "", [], [], []);


        public static DomainException For(string message, Exception? exception = null) =>
            new(message, exception, "", "", [], [], []);

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
        public static DomainException From(Result result, string message = "", Exception? exception = null)
        {
            if (result == null) throw new ArgumentNullException(nameof(result));
            return For(message, exception)
                .WithMessageType(result.FriendlyMessage.MessageType)
                .AddErrorMessages([.. result.Messages])
                .AddValidationMessages([.. result.ValidationMessages])
                .WithCode($"{result.FriendlyMessage.Code}")
                .WithDescription(result.FriendlyMessage.Description)
                .ToDomainException();
        }

        public DomainException AddErrorMessages(params IReadOnlyCollection<FriendlyResultMessage> errors)
        {
            if (errors == null) throw new ArgumentNullException(nameof(errors));

            foreach (var message in errors)
            {
                ErrorMessages.Add(message);
            }

            return this;
        }

        public DomainException AddValidationMessages(params IReadOnlyCollection<ValidationFailure> failures)
        {
            if (failures == null) throw new ArgumentNullException(nameof(failures));
            foreach (var message in failures)
            {
                ValidationFailures.Add(message);
            }

            return this;
        }


        public DomainException AddExceptionMessages(params IReadOnlyCollection<InternalResultMessage> errors)
        {
            if (errors == null) throw new ArgumentNullException(nameof(errors));
            foreach (var message in errors)
            {
                ExceptionErrors.Add(message);
            }

            return this;
        }
    }
}