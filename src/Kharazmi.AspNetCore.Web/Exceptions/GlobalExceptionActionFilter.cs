using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Kharazmi.AspNetCore.Core.Exceptions;
using Kharazmi.AspNetCore.Core.Extensions;
using Kharazmi.AspNetCore.Core.Functional;
using Kharazmi.AspNetCore.Core.Notifications;
using Kharazmi.AspNetCore.Web.Extensions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;

namespace Kharazmi.AspNetCore.Web.Exceptions
{
    /// <summary>
    /// 
    /// </summary>
    public class GlobalExceptionActionFilter : ExceptionFilterAttribute
    {
        private readonly IServiceCollection _services;
        private const string DefaultErrorViewName = "Error";
        private const string DefaultActionName = "Index";
        private const string DefaultControllerName = "Messages";

        private ILogger<GlobalExceptionActionFilter> _logger;
        private ITempDataDictionaryFactory _tempDataDictionaryFactory;
        private IModelMetadataProvider _modelMetadataProvider;

        /// <summary>
        /// 
        /// </summary>
        public RedirectResultExceptionOptions RedirectResultExceptionOptions { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ViewResultExceptionOptions ViewResultExceptionOptions { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool UseNotification { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        public GlobalExceptionActionFilter(IServiceCollection services)
        {
            _services = services;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task OnExceptionAsync(ExceptionContext context)
        {
            _logger = _services.BuildServiceProvider().GetService<ILogger<GlobalExceptionActionFilter>>();
            _tempDataDictionaryFactory = _services.BuildServiceProvider().GetService<ITempDataDictionaryFactory>();
            _modelMetadataProvider = _services.BuildServiceProvider().GetService<IModelMetadataProvider>();
            return ProcessExceptionAsync(context);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        protected virtual Task ProcessExceptionAsync(ExceptionContext context)
        {
            HandleGlobalException(context);
            HandleDomainException(context);

            if (!(context.ActionDescriptor is ControllerActionDescriptor))
            {
                base.OnException(context);
                return Task.CompletedTask;
            }

            var actionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;
            Type controllerType = actionDescriptor?.ControllerTypeInfo;
            var controllerBase = typeof(ControllerBase);
            var controller = typeof(Controller);
            var isAjax = context.HttpContext.Request.IsAjaxRequest();
            var tempData = _tempDataDictionaryFactory.GetTempData(context.HttpContext);

            SelectHandlerException(context);

            // Api
            if (IsApiController(controllerType, controllerBase, controller))
            {
            }

            // Mvc
            if (IsMvcController(controllerType, controllerBase, controller))
            {
                HandleModelState(context);

                if (!isAjax)
                {
                    base.OnException(context);

                    if (ViewResultExceptionOptions != null)
                    {
                        var viewResult = new ViewResult
                        {
                            ViewName = ViewResultExceptionOptions.ErrorViewName.IsEmpty()
                                ? DefaultErrorViewName
                                : ViewResultExceptionOptions.ErrorViewName,
                            TempData = tempData,
                            ViewData = new ViewDataDictionary(_modelMetadataProvider, context.ModelState)
                        };

                        var objectResult = context.Result as BadRequestObjectResult;
                        var result = objectResult?.Value as Result;

                        if (UseNotification)
                        {
                            if (result != null)
                            {
                                tempData.AddNotification(NotificationOptions.For(NotificationType.Error,
                                    result.FriendlyMessage.Description, result.ResultType));

                                foreach (var resultMessage in result.Messages)
                                {
                                    tempData.AddNotification(NotificationOptions.For(NotificationType.Error,
                                        resultMessage.Description, resultMessage.MessageType));
                                }

                                foreach (var validationMessage in result.ValidationMessages)
                                {
                                    tempData.AddNotification(NotificationOptions.For(NotificationType.Validation,
                                        validationMessage.ErrorMessage, validationMessage.PropertyName));
                                }
                            }

                            viewResult.ViewData.Add(
                                NotificationConstant.Notifications,
                                tempData[NotificationConstant.NotificationKey]
                            );
                        }

                        viewResult.ViewData.Model = result;
                        context.ExceptionHandled = true;
                        context.Result = viewResult;
                    }

                    if (RedirectResultExceptionOptions != null)
                    {
                        var redirectResult = RedirectResultExceptionOptions.ActionName.IsEmpty() ||
                                             RedirectResultExceptionOptions.ControllerName.IsEmpty()
                            ? new RedirectToActionResult(DefaultActionName, DefaultControllerName,
                                new { isAjaxRequest = false })
                            : new RedirectToActionResult(RedirectResultExceptionOptions.ActionName,
                                RedirectResultExceptionOptions.ControllerName,
                                RedirectResultExceptionOptions.RouteData);
                        context.Result = redirectResult;
                    }
                }
                else
                {
                    context.HttpContext.Response.ContentType = "application/json; charset=utf-8";
                }
            }


            Dispose(context, tempData);

            return Task.CompletedTask;
        }

        private void SelectHandlerException(ExceptionContext context)
        {
            switch (context.Exception)
            {
                case ValidationException validationException:
                    HandleValidationError(context, validationException);
                    break;
                case DomainException domainException:
                    HandleDomainError(context, domainException);
                    break;
                default:
                    HandleProblemDetails(context);
                    break;
            }
        }


        protected virtual void HandleGlobalException(ExceptionContext context)
        {
            var ex = context?.HttpContext.Features.Get<IExceptionHandlerFeature>()?.Error;
            var errors = ex?.AsJsonException();
            if (errors is null) return;

            _logger.LogError("Global Exception: {JsonException}", errors);
        }

        protected virtual void HandleDomainException(ExceptionContext context)
        {
            switch (context.Exception)
            {
                case DomainException domainException:
                {
                    _logger.LogError("Domain exception {Code} , {Message}",
                        domainException.Code, domainException.Message);

                    foreach (var exception in domainException.ExceptionErrors)
                        _logger.LogError("Domain exception  {ErrorMessage}", exception.ToString());
                    break;
                }
                case UserFriendlyException userFriendlyException:
                {
                    _logger.LogError("Domain exception {Code} , {Message}",
                        userFriendlyException.Code, userFriendlyException.Message);

                    foreach (var exception in userFriendlyException.ErrorMessages)
                        _logger.LogError("UserFriendly exception {Code} , {Description}",
                            exception.Code, exception.Description);
                    break;
                }
                case ValidationException validationException:
                {
                    _logger.LogError("Validation exception {Code} , {Message}",
                        validationException.Code, validationException.Message);

                    foreach (var exception in validationException.Failures)
                        _logger.LogError("Validation exception {PropertyName} , {ErrorMessage}",
                            exception.PropertyName, exception.ErrorMessage);
                    break;
                }
                default:

                    if (context.Exception is FrameworkException frameworkException)
                    {
                        _logger.LogError("Framework exception {Code} , {Message}",
                            frameworkException.Code, frameworkException.Message);
                        break;
                    }

                    break;
            }
        }

        protected virtual void HandleModelState(ExceptionContext context)
        {
            switch (context.Exception)
            {
                case DomainException executeResult:
                {
                    foreach (var message in executeResult.ValidationFailures)
                    {
                        context.ModelState.AddModelError(message.PropertyName, message.ErrorMessage);
                        _logger.LogError("Model state {PropertyName} , {ErrorMessage}",
                            message.PropertyName, message.ErrorMessage);
                    }

                    break;
                }
                case ValidationException validationException:
                    foreach (var message in validationException.Failures)
                    {
                        context.ModelState.AddModelError(message.PropertyName, message.ErrorMessage);
                        _logger.LogError("Model state {PropertyName} , {ErrorMessage}",
                            message.PropertyName, message.ErrorMessage);
                    }

                    break;
            }
        }

        protected virtual void HandleDomainError(ExceptionContext context, DomainException domainException)
        {
            context.Result = new BadRequestObjectResult(
                Result.Fail(domainException.Description)
                    .WithMessages([.. domainException.ErrorMessages])
                    .WithValidationMessages([.. domainException.ValidationFailures])
                    .WithStatus(StatusCodes.Status400BadRequest)
                    .WithRequestPath(context.HttpContext.Request.Path)
                    .WithTraceId(Activity.Current?.Id ?? context.HttpContext.TraceIdentifier))
            {
                ContentTypes = new MediaTypeCollection
                {
                    new MediaTypeHeaderValue("application/json")
                }
            };
            context.HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            context.ExceptionHandled = true;
        }


        protected virtual void HandleValidationError(ExceptionContext context,
            ValidationException validationException)
        {
            if (validationException == null) throw new ArgumentNullException(nameof(validationException));
            context.Result = new BadRequestObjectResult(Result
                .Fail(validationException.Description)
                .WithValidationMessages([.. validationException.Failures])
                .WithStatus(StatusCodes.Status400BadRequest)
                .WithRequestPath(context.HttpContext.Request.Path)
                .WithTraceId(Activity.Current?.Id ?? context.HttpContext.TraceIdentifier))
            {
                ContentTypes = new MediaTypeCollection
                {
                    new MediaTypeHeaderValue("application/json")
                }
            };
            if (context.HttpContext != null)
                context.HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            context.ExceptionHandled = true;
        }

        protected virtual void HandleProblemDetails(ExceptionContext context)
        {
            if (context.Exception is DomainException or ValidationException) return;


            context.Result = new BadRequestObjectResult(
                Result.Fail("ServerError")
                    .WithStatus(StatusCodes.Status400BadRequest)
                    .WithRequestPath(context.HttpContext.Request.Path)
                    .WithTraceId(Activity.Current?.Id ?? context.HttpContext.TraceIdentifier))
            {
                ContentTypes = new MediaTypeCollection
                {
                    new MediaTypeHeaderValue("application/json")
                }
            };
            context.HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            context.ExceptionHandled = true;
        }

        protected virtual void ClearNotification(ITempDataDictionary tempData)
        {
            if (tempData == null) throw new ArgumentNullException(nameof(tempData));
            tempData.Remove(NotificationConstant.NotificationKey);
        }


        protected virtual void Dispose(ExceptionContext context, ITempDataDictionary tempData)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            ClearNotification(tempData);

            switch (context.Exception)
            {
                case ValidationException validationException:
                    validationException.Dispose();
                    break;
            }
        }

        private static bool IsMvcController(Type controllerType, Type controllerBase, Type controller)
        {
            return controllerType != null &&
                   controllerType.IsSubclassOf(controllerBase) &&
                   controllerType.IsSubclassOf(controller);
        }

        private static bool IsApiController(Type controllerType, Type controllerBase, Type controller)
        {
            return controllerType != null &&
                   controllerType.IsSubclassOf(controllerBase) &&
                   !controllerType.IsSubclassOf(controller);
        }
    }
}