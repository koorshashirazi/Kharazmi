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
        public HashSet<ValidationFailure> Failures { get; protected set; } = [];


        /// <summary>
        /// 
        /// </summary>
        protected ValidationException() : this("", null, "", "", [])
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
            string message, Exception? innerException,
            string description, string code,
            HashSet<ValidationFailure> errors) : base(message, innerException, description, code)
        {
            Failures = errors;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static ValidationException Empty() => new("", null, "", "", []);


        public static ValidationException For(string message, Exception? exception = null) => new(message, exception, "", "", []);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        /// <returns></returns>
        public static ValidationException From(Result result, string message = "", Exception? exception = null)
        {
            if (result == null) throw new ArgumentNullException(nameof(result));
            
            return For(message, exception)
                .AddFailureMessages([.. result.ValidationMessages])
                .WithCode($"{result.FriendlyMessage.Code}")
                .WithDescription(result.FriendlyMessage.Description)
                .ToValidationException();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="failures"></param>
        /// <returns></returns>
        public ValidationException AddFailureMessages(params ValidationFailure [] failures)
        {
            if (failures == null) throw new ArgumentNullException(nameof(failures));
            foreach (var failure in failures)
            {
                Failures.Add(failure);
            }

            return this;
        }
    }
}