using Kharazmi.AspNetCore.Core.GuardToolkit;
using Kharazmi.AspNetCore.Core.HandlerRetry;
using Kharazmi.AspNetCore.Core.Pipelines;
using Kharazmi.AspNetCore.Core.Threading;
using Microsoft.Extensions.DependencyInjection;

namespace Kharazmi.AspNetCore.Core.Bus
{
    /// <summary>
    /// 
    /// </summary>
    public static class BusBuilderExtensions
    {
 
        /// <summary>
        /// 
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static PipelineBuilder WithRetryPipeline(this BusBuilder builder, IRetryConfig config)
        {
            Ensure.ArgumentIsNotNull(config, nameof(config));
            var handlerRetryConfig = new HandlerRetryConfig
            {
                Attempt = config.Attempt,
                Max = config.Max,
                Min = config.Min
            };

            builder.Services.AddSingleton<IHandlerRetryConfig>(handlerRetryConfig);
            builder.Services.AddSingleton<IHandlerTransientRetryDelay, HandlerTransientRetryDelay>();
            builder.Services.AddSingleton<IHandlerRetryStrategy, HandlerRetryStrategy>();

            return new PipelineBuilder(builder);
        }
    }
}