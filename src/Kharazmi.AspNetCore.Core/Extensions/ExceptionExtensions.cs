using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Kharazmi.AspNetCore.Core.Exceptions;
using Kharazmi.AspNetCore.Core.Functional;
using Newtonsoft.Json;

namespace Kharazmi.AspNetCore.Core.Extensions
{
    public static partial class Core
    {
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
            public IEnumerable<MessageModel?> ExceptionErrors { get; set; }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IEnumerable<MessageModel?> CollectExceptionIncludeInnerException<T>(this T e)
            where T : Exception, new()
        {
            var exceptionErrors = new List<MessageModel?>();
            while (true)
            {
                if (e == null) return exceptionErrors;
                exceptionErrors.Add(MessageModel.For(e.Message, typeof(T).Name));
                if (e.InnerException == null) return exceptionErrors;
                exceptionErrors.Add(MessageModel.For("\r\nInnerException: " + e.InnerException.Message,
                    e.GetType().Name));
                e = (T) e.InnerException;
            }
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="exception"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T WithDetailsJsonException<T>(this T exception) where T : Exception, new()
        {
            var stacktrace = (new StackTrace(exception, true).GetFrames() ??
                              new[] {new StackFrame(1, true)})
                .Where(x => x.GetFileName() != null);

            var stackFrameModels = (from stackFrame in stacktrace
                let methodBase = stackFrame.GetMethod()
                let fileName = stackFrame.GetFileName()
                let lineNumber = stackFrame.GetFileLineNumber()
                let memberInfo = methodBase?.DeclaringType
                select new StackFrameModel
                {
                    FileName = fileName,
                    ClassName = memberInfo?.FullName,
                    MethodName = memberInfo?.Name,
                    LinNumber = lineNumber
                }).ToList();

            var details = new DetailsError
            {
                StackFrameModel = stackFrameModels,
                CurrentDate = DateTime.Now.ToShortDateString(),
                ExceptionErrors = exception.CollectExceptionIncludeInnerException()
            };

            var jsonDetails = JsonConvert.SerializeObject(details, Formatting.Indented, new JsonSerializerSettings
            {
                MaxDepth = 10,
                NullValueHandling = NullValueHandling.Ignore
            });

            return (T) Activator.CreateInstance(typeof(T), jsonDetails);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="exception"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T WithDetailsException<T>(this T exception) where T : Exception, new()
        {
            var exceptionMessage = new StringBuilder();
            var frame = new StackFrame(1, true);
            var methodBase = frame.GetMethod();
            var fileName = frame.GetFileName();
            var lineNumber = frame.GetFileLineNumber();
            var currentDateTime = DateTime.Now.ToShortDateString();

            var memberInfo = methodBase?.DeclaringType;
            var exceptionErrors = exception.CollectExceptionIncludeInnerException();

            exceptionMessage.Append(LineLogSeparator).AppendLine();

            exceptionMessage.Append($"Start an error has occurred: {currentDateTime}").AppendLine();

            exceptionMessage.Append($"Exception Type: {typeof(T).Name}").AppendLine();

            exceptionMessage.Append($"Class: {memberInfo?.FullName ?? ""}").AppendLine();
            exceptionMessage.Append($"Method: {memberInfo?.Name}").AppendLine();
            exceptionMessage.Append($"FileName: {fileName}").AppendLine();
            exceptionMessage.Append($"LineNumber: {lineNumber}").AppendLine();


            foreach (var MessageModel in exceptionErrors)
            {
                exceptionMessage.Append($"Exception Message:").AppendLine();
                exceptionMessage.Append($"Exception Message NotificationId: {MessageModel.Code}").AppendLine();
                exceptionMessage.Append($"Exception Message Description: {MessageModel.Description}").AppendLine();
            }

            exceptionMessage.Append($"End of exception: {LineLogSeparator}").AppendLine();


            return (T) Activator.CreateInstance(typeof(T), exceptionMessage.ToString());
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