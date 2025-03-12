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
    public class ValidationException : FrameworkException
    {
        /// <summary> </summary>
        public IReadOnlyCollection<ValidationFailure> Failures { get; protected set; }


        /// <summary>
        /// 
        /// </summary>
        protected ValidationException() : this("", null, "", "", Enumerable.Empty<ValidationFailure>())
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="code"></param>
        /// <param name="innerException"></param>
        /// <param name="errors"></param>
        /// <param name="description"></param>
        /// <param name="message"></param>
        protected ValidationException(
            string message, Exception innerException,
            string description, string code,
            IEnumerable<ValidationFailure> errors) : base(message, innerException, description, code)
        {
            Failures = errors.AsReadOnly();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static ValidationException Empty() =>
            new ValidationException("", null, "", "", Enumerable.Empty<ValidationFailure>());


        public static ValidationException For(string message, Exception exception = null) =>
            new ValidationException(message, exception, "", "", Enumerable.Empty<ValidationFailure>());


        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        /// <returns></returns>
        public static ValidationException From(Result result, string message = "", Exception exception = null)
        {
            return For(message, exception)
                .AddFailureMessages(result?.ValidationMessages)
                .WithCode(result?.Code)
                .WithDescription(result?.Description)
                .ToValidationException();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="failure"></param>
        /// <returns></returns>
        public ValidationException AddFailureMessage(ValidationFailure failure)
        {
            if (Failures == null)
                Failures = new List<ValidationFailure>();

            if (failure != null)
            {
                var failureMessages = Failures.ToList();
                failureMessages.Add(failure);
                Failures = failureMessages.AsReadOnly();
            }

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="failures"></param>
        /// <returns></returns>
        public ValidationException AddFailureMessages(IEnumerable<ValidationFailure> failures)
        {
            if (Failures == null)
                Failures = new List<ValidationFailure>();

            if (failures != null)
            {
                var failureMessages = Failures.ToList();
                failureMessages.AddRange(failures);
                Failures = failureMessages.AsReadOnly();
            }

            return this;
        }
    }
}