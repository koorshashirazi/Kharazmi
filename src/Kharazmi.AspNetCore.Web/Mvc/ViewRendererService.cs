using System;
using System.IO;
using System.Threading.Tasks;
using Kharazmi.AspNetCore.Core.Extensions;
using Kharazmi.AspNetCore.Core.Threading;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Kharazmi.AspNetCore.Web.Mvc
{
    /// <summary>
    /// 
    /// </summary>
    public static class RazorViewToStringRendererExtensions
    {
        /// <summary>
        /// Adds IViewRendererService to IServiceCollection.
        /// </summary>
        public static IServiceCollection AddRazorViewRenderer(this IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddScoped<IViewRenderService, ViewRenderService>();
            return services;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public interface IViewRenderService
    {
        /// <summary>
        /// Renders a .cshtml file as an string.
        /// </summary>
        Task<(bool HasView, string Content)> RenderViewToStringAsync(string viewName, object model = null,
            ModelStateDictionary modelState = null);

        /// <summary>
        /// Renders a .cshtml file as an string.
        /// </summary>
        /// <param name="viewName"></param>
        /// <param name="model"></param>
        /// <param name="modelState"></param>
        /// <returns></returns>
        (bool HasView, string Content) RenderViewToString(string viewName, object model = null,
            ModelStateDictionary modelState = null);
    }

    /// <summary>
    ///
    /// </summary>
    public class ViewRenderService : IViewRenderService
    {
        private readonly ILogger<ViewRenderService> _logger;
        private readonly IRazorViewEngine _razorViewEngine;
        private readonly ITempDataProvider _tempDataProvider;
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// Renders a .cshtml file as an string.
        /// </summary>
        public ViewRenderService(
            IRazorViewEngine razorViewEngine,
            ITempDataProvider tempDataProvider,
            IServiceProvider serviceProvider,
            ILogger<ViewRenderService> logger)
        {
            _razorViewEngine = razorViewEngine;
            _tempDataProvider = tempDataProvider;
            _serviceProvider = serviceProvider;
            _tempDataProvider.CheckArgumentIsNull(nameof(_tempDataProvider));
            _logger = logger;
            _logger.CheckArgumentIsNull(nameof(_logger));
        }


        /// <summary>
        /// Renders a .cshtml file as an string.
        /// </summary>
        public async Task<(bool HasView, string Content)> RenderViewToStringAsync(string viewNamePath, object model,
            ModelStateDictionary modelState = null)
        {
            using var requestServices = _serviceProvider.CreateScope();
            var httpAccess = requestServices.ServiceProvider.GetService<IHttpContextAccessor>();

            var httpContext = httpAccess.HttpContext ?? new DefaultHttpContext
                                  {RequestServices = requestServices.ServiceProvider};

            var routeData = httpContext.GetRouteData() ?? new RouteData();

            var actionContext = new ActionContext(httpContext, routeData, new ActionDescriptor());

            using var sw = new StringWriter();

            var viewResult = viewNamePath.EndsWith(".cshtml")
                ? _razorViewEngine.GetView(viewNamePath, viewNamePath, false)
                : _razorViewEngine.FindView(actionContext, viewNamePath, false);

            if (viewResult.View == null)
            {
                _logger.LogError($"{viewNamePath} does not match any available view");
                return (false, $"{viewNamePath} does not match any available view");
            }

            var viewDictionary =
                new ViewDataDictionary(new EmptyModelMetadataProvider(), modelState ?? new ModelStateDictionary())
                {
                    Model = model
                };

            var viewContext = new ViewContext(
                actionContext,
                viewResult.View,
                viewDictionary,
                new TempDataDictionary(actionContext.HttpContext, _tempDataProvider),
                sw,
                new HtmlHelperOptions()
            ) {RouteData = routeData};
            try
            {
                await viewResult.View.RenderAsync(viewContext).ConfigureAwait(false);
                return (true, sw.ToString());
            }
            catch (Exception ex)
            {
                _logger.LogError("Render View Exception: @{Exception}", ex.Message);

                return (false, $"can not render view with Model: {nameof(model)}");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="viewName"></param>
        /// <param name="model"></param>
        /// <param name="modelState"></param>
        /// <returns></returns>
        public (bool HasView, string Content) RenderViewToString(string viewName, object model = null,
            ModelStateDictionary modelState = null)
        {
            return AsyncHelper.RunSync(() => RenderViewToStringAsync(viewName, model, modelState));
        }
    }
}