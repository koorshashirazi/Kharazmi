using Kharazmi.AspNetCore.Core.AjaxPaging;
using Kharazmi.AspNetCore.Core.GuardToolkit;
using Kharazmi.AspNetCore.Core.Runtime;
using Kharazmi.AspNetCore.Web.Authorization;
using Kharazmi.AspNetCore.Web.Hosting;
using Kharazmi.AspNetCore.Web.Http;
using Kharazmi.AspNetCore.Web.Localizations;
using Kharazmi.AspNetCore.Web.Mail;
using Kharazmi.AspNetCore.Web.Mvc;
using Kharazmi.AspNetCore.Web.Runtime;
using Kharazmi.AspNetCore.Web.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Kharazmi.AspNetCore.Web
{
    /// <summary>
    /// Configure Kharazmi.AspNetCore.Web services
    /// </summary>
    public class WebBuilder
    {
        /// <summary> </summary>
        public IServiceCollection Services { get; }

        /// <summary> </summary>
        /// <param name="services"></param>
        public WebBuilder(IServiceCollection services)
        {
            Services = services;
        }

        /// <summary>
        /// Register IAntiXssService
        /// To use this feature, you can call <see cref="IAntiXssService"/> 
        /// </summary>
        /// <returns></returns>
        public WebBuilder WithAntiXss()
        {
            Services.AddAntiXssService();
            return this;
        }

        /// <summary>
        /// Register IUploadFileService
        /// A service for uploading files
        /// To use this feature, you can call <see cref="IUploadFileService"/> 
        /// </summary>
        /// <returns></returns>
        public WebBuilder WithUploadFile()
        {
            Services.AddUploadFileService();
            return this;
        }

        /// <summary>
        /// Register IHttpRequestInfoService
        /// All information about http request
        /// To use this feature, you can call <see cref="IHttpRequestInfoService"/> 
        /// </summary>
        /// <returns></returns>
        public WebBuilder WithHttpContextInfo()
        {
            Services.AddHttpRequestInfoService();
            return this;
        }

        /// <summary>
        /// Register IViewRenderService
        /// Render cshtml to string
        /// To use this feature, you can call <see cref="IViewRenderService"/> 
        /// </summary>
        /// <returns></returns>
        public WebBuilder WithRazorViewRenderer()
        {
            Services.AddRazorViewRenderer();
            return this;
        }

        /// <summary>
        /// Register IFileNameSanitizerService
        /// To use this feature, you can call <see cref="IFileNameSanitizerService"/> 
        /// </summary>
        /// <returns></returns>
        public WebBuilder WithFileNameSanitizer()
        {
            Services.AddFileNameSanitizerService();
            return this;
        }

        /// <summary>
        /// Register IMvcDiscoveryService
        /// To use this feature, you can call <see cref="IMvcDiscoveryService"/> 
        /// </summary>
        /// <returns></returns>
        public WebBuilder WithMvcDiscovery()
        {
            Services.AddMvcDiscoveryService();
            return this;
        }

        /// <summary>
        /// Generate Url from Url.Action()
        /// To use this feature, you can call <see cref="IMvcUrlService"/> 
        /// </summary>
        /// <returns></returns>
        public WebBuilder WithMvcUrlService()
        {
            Services.AddMvcUrlService();
            return this;
        }


        /// <summary> </summary>
        /// <returns></returns>
        public WebBuilder WithBackgroundLongRunning()
        {
            Services.AddHostedService<QueuedHostedService>();
            return this;
        }

        /// <summary>To use this feature, you can call <see cref="IUserSession"/> </summary>
        /// <returns></returns>
        public WebBuilder WithUserSession()
        {
            Services.AddScoped<IUserSession, UserSession>();
            return this;
        }

        /// <summary>To use this feature, you can call <see cref="IBuildPaginationLinks"/> </summary>
        /// <returns></returns>
        public WebBuilder WithPagination()
        {
            Services.TryAddTransient<IBuildPaginationLinks, PaginationLinkBuilder>();
            return this;
        }

        /// <summary>To use this feature, you can call <see cref="IHtmlHelperService"/> </summary>
        /// <returns></returns>
        public WebBuilder WithHtmlHelper()
        {
            Services.AddHtmlHelperService();
            return this;
        }

        /// <summary>
        /// Send and Receive email
        /// To use this feature, you can call <see cref="IEmailService"/>
        /// </summary>
        /// <returns></returns>
        public WebBuilder WithWebMail(EmailServerOptions options)
        {
            Ensure.ArgumentIsNotNull(options, nameof(options));
            Services.AddEmailServices(options);
            return this;
        }


        /// <summary>To use this feature, you can call <see cref="Microsoft.AspNetCore.Authorization.IAuthorizationPolicyProvider"/> </summary>
        /// <returns></returns>
        public WebBuilder WithDynamicPolicy()
        {
            Services.AddSingleton<IAuthorizationPolicyProvider, AuthorizationPolicyProvider>();
            return this;
        }

        /// <summary> </summary>
        /// <returns></returns>
        public WebBuilder WithHttpClientFactory()
        {
            Services.AddCommonHttpClientFactory();
            return this;
        }  
        
        /// A service that provides information about the current user
        /// userId, userName, email and Culture...
        /// To use this feature, you can call <see cref="IUserContextService"/>
        public WebBuilder WithUserContext()
        {
            Services.AddUserContextService();
            return this;
        }
       
        /// A service to use a shared resource throughout the application
        /// userId, userName, email and Culture...
        /// To use this feature, you can call <see cref="ISharedLocalizationService"/>
        public WebBuilder WithISharedLocalization(ResourceOptions options)
        {
            Services.AddLocalizationService(options);
            return this;
        }
    }
}