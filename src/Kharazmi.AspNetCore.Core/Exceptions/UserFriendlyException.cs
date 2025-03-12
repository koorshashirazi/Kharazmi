using System;
using System.Collections.Generic;
using System.Linq;
using Kharazmi.AspNetCore.Core.Extensions;
using Kharazmi.AspNetCore.Core.Functional;

namespace Kharazmi.AspNetCore.Core.Exceptions
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class UserFriendlyException : FrameworkException
    {
        /// <summary></summary>
        public IReadOnlyCollection<MessageModel?> ErrorMessages { get; protected set; }

        private UserFriendlyException(string message, Exception exception, string description, string code,
            IEnumerable<MessageModel?> errorMessages) : base(
            message, exception, description, code)
        {
            ErrorMessages = errorMessages.AsReadOnly();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static UserFriendlyException Empty() =>
            new UserFriendlyException("", null, "", "", Enumerable.Empty<MessageModel>());


        public static UserFriendlyException For(string message, Exception exception = null) =>
            new UserFriendlyException(message, exception, "", "", Enumerable.Empty<MessageModel>());


        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        /// <returns></returns>
        public static UserFriendlyException From(Result result, string message = "", Exception exception = null)
        {
            return For(message, exception)
                .AddErrorMessages(result?.Messages)
                .WithCode(result?.Code)
                .WithDescription(result?.Description)
                .ToUserFriendlyException();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="error"></param>
        /// <returns></returns>
        public UserFriendlyException AddErrorMessage(MessageModel? error)
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="errors"></param>
        /// <returns></returns>
        public UserFriendlyException AddErrorMessages(IEnumerable<MessageModel?> errors)
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
    }
}