using System;
using System.Reflection;
using Kharazmi.AspNetCore.Core.Bus;
using Kharazmi.AspNetCore.Core.Dependency;
using Kharazmi.AspNetCore.Core.Dispatchers;
using Kharazmi.AspNetCore.Core.Domain;
using Kharazmi.AspNetCore.Core.Domain.Aggregates;
using Kharazmi.AspNetCore.Core.EventSourcing;
using Kharazmi.AspNetCore.Core.Extensions;
using Kharazmi.AspNetCore.Core.GuardToolkit;
using Kharazmi.AspNetCore.Core.Handlers;
using Kharazmi.AspNetCore.Core.Timing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Kharazmi.AspNetCore.Core
{
    /// <summary>
    ///
    /// </summary>
    public static class CoreFrameworkBuilderExtensions
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static CoreFrameworkBuilder AddCoreFramework(this IServiceCollection services)
        {
            Ensure.ArgumentIsNotNull(services, nameof(services));

            services.AddTransient<IClock, Clock>();
            services.AddTransient(typeof(Lazy<>), typeof(LazyFactory<>));

            return new CoreFrameworkBuilder(services);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static BusBuilder AddEventBus(
            this CoreFrameworkBuilder builder,
            Assembly[]? assemblies = null,
            ServiceLifetime dispatcherFactoryLifeTime = ServiceLifetime.Scoped,
            ServiceLifetime dispatcherLifeTime = ServiceLifetime.Scoped,
            ServiceLifetime domainContextService = ServiceLifetime.Scoped)
        {
            Ensure.ArgumentIsNotNull(builder, nameof(builder));
            builder.Services.RegisterHandlers(ServiceLifetime.Transient, assemblies);
            builder.Services.TryAddScoped<IDomainNotificationHandler, DomainNotificationHandler>();

            builder.Services.TryAddService<IDomainContextService, DomainContextService>(domainContextService);

            builder.Services.AddService(sp => new EventSourcingOptions(), ServiceLifetime.Singleton);

            builder.Services.AddTransient<IJsonSerializer, JsonSerializer>();

            builder.Services.AddTransient<IEventSerializer, JsonEventSerializer>();

            builder.Services.AddTransient<IDomainEventFactory, DomainEventFactory>();

            builder.Services.AddTransient<IInstanceCreator, InstanceCreator>();
            builder.Services.AddScoped<IAggregateFactory, AggregateFactory>();

            builder.Services.TryAddService<IEventStore, NullEventStore>(ServiceLifetime.Scoped);

            builder.Services.AddDispatchers(dispatcherLifeTime, dispatcherFactoryLifeTime);

            return new BusBuilder(builder);
        }
    }
}