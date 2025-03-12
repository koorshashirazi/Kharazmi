using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;

namespace Kharazmi.AspNetCore.Web.Mvc
{
    /// <summary>  /// </summary>
    public interface IMvcUrlService
    {
        /// <summary>
        /// 
        /// </summary>
        IUrlHelper Url { get; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class MvcUrlService : IMvcUrlService
    {
        private readonly IServiceProvider serviceProvider;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceProvider"></param>
        public MvcUrlService(IServiceProvider serviceProvider)
        {

            this.serviceProvider = serviceProvider;
        }

        /// <summary>
        /// 
        /// </summary>
        public IUrlHelper Url
        {
            get
            {
                using var requestServices = serviceProvider.CreateScope();
                var httpAccess = requestServices.ServiceProvider.GetService<IHttpContextAccessor>();

                var httpContext = httpAccess.HttpContext ?? new DefaultHttpContext
                { RequestServices = requestServices.ServiceProvider };

                var routeData = httpContext.GetRouteData() ?? new RouteData();

                var actionContext = new ActionContext(httpContext, routeData, new ActionDescriptor());
                var urlFactory = requestServices.ServiceProvider.GetService<IUrlHelperFactory>();
                return urlFactory.GetUrlHelper(actionContext);
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public static class MvcUrlServiceExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddMvcUrlService(this IServiceCollection services)
        {
            services.TryAddScoped<IMvcUrlService, MvcUrlService>();
            return services;
        }
    }
}