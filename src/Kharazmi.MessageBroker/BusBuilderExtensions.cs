using Kharazmi.AspNetCore.Core.Bus;
using Kharazmi.AspNetCore.Core.Domain;
using Kharazmi.AspNetCore.Core.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RawRabbit;
using RawRabbit.Common;
using RawRabbit.Configuration;
using RawRabbit.Enrichers.MessageContext;
using RawRabbit.Instantiation;

namespace Kharazmi.MessageBroker
{
    /// <summary>
    /// 
    /// </summary>
    public static class BusBuilderExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IBusSubscriber UseRabbitMq(this IApplicationBuilder app)
            => new BusSubscriber(app);


        /// <summary> Add bus manager to bus</summary>
        internal static BusBuilder WithBusManager(this BusBuilder builder, ServiceLifetime serviceLifetime)
        {
            builder.Services.TryAddService<IBusPublisher, BusPublisher>(serviceLifetime);
            builder.Services.TryAddService<IBusManager, BusManager>(serviceLifetime);
            return builder;
        }

        /// <summary></summary>
        /// <param name="builder"></param>
        /// <param name="serviceLifetime"></param>
        /// <returns></returns>
        public static BusBuilder WithBusHandler(this BusBuilder builder,
            ServiceLifetime serviceLifetime = ServiceLifetime.Transient)
        {
            builder.Services.AddService<IBusHandler, BusHandler>(serviceLifetime);
            return builder;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="rabbitMqOptions"></param>
        /// <param name="busManagerLifetime"></param>
        /// <param name="busClientServiceLifeTime"></param>
        /// <returns></returns>
        public static BusBuilder AddRabbitMq(
            this BusBuilder builder,
            RabbitMqOptions rabbitMqOptions = null, 
            ServiceLifetime busManagerLifetime = ServiceLifetime.Scoped,
            ServiceLifetime busClientServiceLifeTime = ServiceLifetime.Singleton)
        {
            builder.WithBusManager(busManagerLifetime);
            builder.Services.AddSingleton(sp =>
            {
                if (rabbitMqOptions != null) return rabbitMqOptions;
                var configuration = sp.GetService<IConfiguration>();
                var options = configuration.GetOptions<RabbitMqOptions>("RabbitMqOptions");
                return options;
            });


            builder.Services.AddSingleton<RawRabbitConfiguration>(sp =>
            {
                if (rabbitMqOptions != null) return rabbitMqOptions;
                var configuration = sp.GetService<IConfiguration>();
                var options = configuration.GetOptions<RabbitMqOptions>("RabbitMqOptions");
                return options;
            });


            builder.Services.AddSingleton<IInstanceFactory>(context =>
            {
                var options = context.GetService<RabbitMqOptions>();
                var configuration = context.GetService<RawRabbitConfiguration>();
                var namingConventions = new CustomNamingConventions(options);

                return RawRabbitFactory.CreateInstanceFactory(new RawRabbitOptions
                {
                    DependencyInjection = ioc =>
                    {
                        ioc.AddSingleton(options);
                        ioc.AddSingleton(configuration);
                        ioc.AddSingleton<INamingConventions>(namingConventions);
                    },
                    Plugins = p => p
                        .UseAttributeRouting()
                        .UseRetryLater()
                        .UpdateRetryInfo()
                        .UseMessageContext<DomainContext>()
                        .UseContextForwarding()
                });
            });
            
            builder.Services.AddService(context => context.GetService<IInstanceFactory>().Create(), busClientServiceLifeTime);
            
            return builder;
        }

        private static IClientBuilder UpdateRetryInfo(this IClientBuilder clientBuilder)
        {
            clientBuilder.Register(c => c.Use<RetryStagedMiddleware>());

            return clientBuilder;
        }

        private static TModel GetOptions<TModel>(this IConfiguration configuration, string section) where TModel : new()
        {
            var model = new TModel();
            configuration.GetSection(section).Bind(model);

            return model;
        }
    }
}