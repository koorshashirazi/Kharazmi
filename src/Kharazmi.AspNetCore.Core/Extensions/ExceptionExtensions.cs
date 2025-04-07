using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Kharazmi.AspNetCore.Core.Exceptions;
using Kharazmi.AspNetCore.Core.Functional;
using Kharazmi.AspNetCore.Core.Models;
using Newtonsoft.Json;

namespace Kharazmi.AspNetCore.Core.Extensions
{
    public static partial class Core
    {
        public static string AsJsonException(this Exception exception)
        {
            var collectExceptions = ExceptionMessage.FromException(exception);
            return JsonConvert.SerializeObject(collectExceptions);
        }
        
        /// <summary>
        /// 
        /// </summary>
        public const string LineLogSeparator =
            @"===================================================================================";


        /// <summary>
        /// 
        /// </summary>
        public class StackFrameModel
        {
            /// <summary> </summary>
            public string FileName { get; set; }

            /// <summary></summary>
            public string ClassName { get; set; }


            /// <summary></summary>
            public string MethodName { get; set; }

            /// <summary></summary>
            public int LinNumber { get; set; }
        }

        /// <summary></summary>
        public class DetailsError
        {
            /// <summary></summary>
            public string CurrentDate { get; set; }

            /// <summary> </summary>
            public IEnumerable<StackFrameModel> StackFrameModel { get; set; }

            /// <summary> </summary>
            public IEnumerable<FrameworkException> ExceptionErrors { get; set; }
        }
     

        /// <summary>
        /// 
        /// </summary>
        /// <param name="o"></param>
        /// <param name="name"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public static object CheckArgumentIsNull(this object o, string name)
        {
            return o switch
            {
                string objString when objString.IsEmpty() => throw new ArgumentNullException(name),
                null => throw new ArgumentNullException(name),
                _ => o,
            };
        }

        public static void AsDomainException(this Exception exception)
        {
            switch (exception)
            {
                case DomainException domainException:
                    throw domainException;
                case ValidationException validationException:
                    var validException = DomainException.For(validationException.Message, validationException)
                        .AddValidationMessages(validationException.Failures)
                        .WithCode(validationException.Code)
                        .WithDescription(validationException.Description).ToDomainException();
                    throw validException;
                case OperationCanceledException canceledException:
                    var operationCanceledException =
                        DomainException.For(canceledException.Message, canceledException);
                    throw operationCanceledException;
                default:
                    var otherExceptions = DomainException.For(exception.Message, exception);
                    throw otherExceptions;
            }
        }

        public static DomainException ToDomainException(this Exception exception)
        {
            switch (exception)
            {
                case DomainException domainException:
                    return domainException;
                case ValidationException validationException:
                    var validException = DomainException.For(validationException.Message, validationException)
                        .AddValidationMessages(validationException.Failures)
                        .WithCode(validationException.Code)
                        .WithDescription(validationException.Description)
                        .ToDomainException();
                    return validException;
                case OperationCanceledException canceledException:
                    var operationCanceledException = DomainException.For(canceledException.Message, canceledException);
                    return operationCanceledException;
                default:
                    var otherExceptions = DomainException.For(exception.Message, exception);
                    return otherExceptions;
            }
        }

        public static DomainException ToDomainException(this FrameworkException exception)
            => exception as DomainException;

        public static UserFriendlyException ToUserFriendlyException(this FrameworkException exception)
            => exception as UserFriendlyException;

        public static ValidationException ToValidationException(this FrameworkException exception)
            => exception as ValidationException;


        public static MessageBusException ToMessageBusException(this FrameworkException exception)
            => exception as MessageBusException;
    }
}