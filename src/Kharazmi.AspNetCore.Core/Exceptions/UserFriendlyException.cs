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
        public HashSet<FriendlyResultMessage> ErrorMessages { get; protected set; }

        private UserFriendlyException(string message, Exception? exception, string description, string code,
            HashSet<FriendlyResultMessage> errorMessages) : base(
            message, exception, description, code)
        {
            ErrorMessages = errorMessages;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static UserFriendlyException Empty() =>
            new ("", null, "", "", []);


        public static UserFriendlyException For(string message, Exception? exception = null) =>
            new (message, exception, "", "", []);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        /// <returns></returns>
        public static UserFriendlyException From(Result result, string message = "", Exception? exception = null)
        {
            return For(message, exception)
                .AddErrorMessages(result.Messages)
                .WithCode($"{result.FriendlyMessage.Code}")
                .WithDescription(result.FriendlyMessage.Description)
                .ToUserFriendlyException();
        }
 

        /// <summary>
        /// 
        /// </summary>
        /// <param name="errors"></param>
        /// <returns></returns>
        public UserFriendlyException AddErrorMessages(params IReadOnlyCollection<FriendlyResultMessage> errors)
        {
            if (errors == null) throw new ArgumentNullException(nameof(errors));
            foreach (var message in errors)
            {
                ErrorMessages.Add(message);
            }

            return this;
        }
    }
}