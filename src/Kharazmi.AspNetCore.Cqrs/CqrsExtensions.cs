using System;
using System.Reflection;
using Kharazmi.AspNetCore.Core.Cqrs;
using Kharazmi.AspNetCore.Core.EventSourcing;
using Kharazmi.AspNetCore.Cqrs.Behaviors;
using Kharazmi.AspNetCore.Cqrs.Handlers;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Kharazmi.AspNetCore.Cqrs
{
    /// <summary>
    /// 
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        /// <param name="handlerType">Type of a Handler in assembly</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static ICqrsBuilder AddCqrs(this IServiceCollection services, Type handlerType)
        {
            services.AddScoped<IDomainNotificationHandler, DomainNotificationHandler>();
            services.AddTransient<IEventStore, NullEventStore>();
            services.AddSingleton<IDomainPublisher, DomainPublisher>();
            services.AddSingleton(_ => new EventSourcingOptions());
            services.AddMediatR(x=> x.RegisterServicesFromAssembly(Assembly.GetAssembly(handlerType)));
            return new CqrsBuilder(services);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static ICqrsBuilder WithLoggingPipeline(this ICqrsBuilder builder)
        {
            builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
            return builder;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static ICqrsBuilder WithValidatorPipeline(this ICqrsBuilder builder)
        {
            builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            return builder;
        }
    }
}