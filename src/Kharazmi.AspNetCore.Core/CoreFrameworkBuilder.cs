using System;
using Kharazmi.AspNetCore.Core.Cryptography;
using Kharazmi.AspNetCore.Core.Http;
using Kharazmi.AspNetCore.Core.Threading.BackgroundTasks;
using Kharazmi.AspNetCore.Core.Validation;
using Kharazmi.AspNetCore.Core.Validation.Interception;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Kharazmi.AspNetCore.Core
{
    /// <summary>
    /// Build different parts of the core framework
    /// </summary>
    public sealed class CoreFrameworkBuilder
    {
        /// <summary>
        ///
        /// </summary>
        public IServiceCollection Services { get; }

        /// <summary>
        ///
        /// </summary>
        /// <param name="services"></param>
        public CoreFrameworkBuilder(IServiceCollection services)
        {
            Services = services;
        }


        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public CoreFrameworkBuilder WithUrlNormalization()
        {
            Services.AddUrlNormalizationService();
            return this;
        }


        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public CoreFrameworkBuilder WithRedirectUrlFinder()
        {
            Services.AddRedirectUrlFinderService();
            return this;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public CoreFrameworkBuilder WithDownloader()
        {
            Services.AddDownloaderService();
            return this;
        }


        /// <summary>
        /// Register the ISecurityService
        /// </summary>
        public CoreFrameworkBuilder WithSecurityService()
        {
            Services.TryAddSingleton<ISecurityService, SecurityService>();
            return this;
        }


        /// <summary>
        /// Register the IBackgroundTaskQueue
        /// </summary>
        public CoreFrameworkBuilder WithBackgroundTaskQueue()
        {
            Services.TryAddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();
            return this;
        }

        /// <summary>
        /// Register the IRandomNumberProvider
        /// </summary>
        public CoreFrameworkBuilder WithRandomNumberProvider()
        {
            Services.TryAddSingleton<IRandomNumberProvider, RandomNumberProvider>();
            return this;
        }

        /// <summary>
        /// Register the validation infrastructure's services
        /// </summary>
        public CoreFrameworkBuilder WithModelValidation(Action<ValidationOptions> setupAction = null)
        {
            Services.TryAddTransient<ValidationInterceptor>();
            Services.TryAddTransient<MethodInvocationValidator>();
            Services.TryAddTransient<IMethodParameterValidator, DataAnnotationMethodParameterValidator>();
            Services.TryAddTransient<IMethodParameterValidator, ValidatableObjectMethodParameterValidator>();
            Services.TryAddTransient<IMethodParameterValidator, ModelValidationMethodParameterValidator>();

            if (setupAction != null)
            {
                Services.Configure(setupAction);
            }

            return this;
        }
    }
}