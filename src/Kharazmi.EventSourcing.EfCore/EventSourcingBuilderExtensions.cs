using System;
using Kharazmi.AspNetCore.Core.Bus;
using Kharazmi.AspNetCore.Core.EventSourcing;
using Kharazmi.AspNetCore.Core.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Kharazmi.EventSourcing.EfCore
{
    /// <summary>
    /// 
    /// </summary>
    public static class EventSourcingBuilderExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="options"></param>
        /// <param name="serviceLifetime"></param>
        /// <returns></returns>
        public static BusBuilder AddEventSourcing(this BusBuilder builder,
            EventSourcingOptions options,
            ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            builder.Services.TryAddService<IEventRequestInfoService, NullEventRequestInfoService>(serviceLifetime);
            builder.Services.TryAddService<IEventUserInfoService, NullEventUserInfoService>(serviceLifetime);
            builder.Services.TryAddService<IEventUserClaimInfoService, NullEventUserClaimInfoService>(serviceLifetime);
            builder.Services.AddService(sp => options, ServiceLifetime.Singleton);
            builder.Services.AddService<IEventStoreUnitOfWork, EventStoreUnitOfWork>(ServiceLifetime.Scoped);

            return builder;
        }

        /// <summary>
        ///  where TEventStore : class, IEventStore<TEventStore/>
        /// </summary>
        /// <returns></returns>
        public static BusBuilder WithDefaultEventStore(
            this BusBuilder builder,
            ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            builder.Services.AddService<IEventStore, SqlEventStore>(serviceLifetime);
            return builder;
        }

        /// <summary>
        ///  where TEventStore : class, IEventStore<TEventStore/>
        /// </summary>
        /// <returns></returns>
        public static BusBuilder WithEventStore<TEventStore>(
            this BusBuilder builder,
            ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
            where TEventStore : class, IEventStore
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            builder.Services.TryAddService<IEventStore, TEventStore>(serviceLifetime);
            return builder;
        }
    }
}