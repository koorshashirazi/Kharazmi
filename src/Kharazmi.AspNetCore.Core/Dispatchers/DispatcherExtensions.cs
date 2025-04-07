using Kharazmi.AspNetCore.Core.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Kharazmi.AspNetCore.Core.Dispatchers
{
    internal static class DispatcherExtensions
    {
        public static IServiceCollection AddDispatchers(this IServiceCollection services,
            ServiceLifetime dispatchersLifeTime,
            ServiceLifetime dispatcherFactoryLifetime)
        {
            services.TryAddService<IDomainCommandDispatcher, DomainCommandDispatcher>(dispatchersLifeTime);
            services.TryAddService<IDomainEventDispatcher, DomainEventDispatcher>(dispatchersLifeTime);
            services.TryAddService<IDomainQueryDispatcher, DomainQueryDispatcher>(dispatchersLifeTime); 
            services.TryAddService<IDomainDispatcher, DomainDispatcher>(dispatcherFactoryLifetime);
            services.TryAddSingleton<IDomainCommandHandlerTypeFactory, DomainCommandHandlerTypeFactory>();
            return services;
        }
    }
}