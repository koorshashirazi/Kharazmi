using Kharazmi.AspNetCore.Core.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Kharazmi.AspNetCore.Core.Dispatchers
{
    internal static class DispatcherExtensions
    {
        public static IServiceCollection AddDispatchers(this IServiceCollection services,
            ServiceLifetime dispatchersLifeTime,
            ServiceLifetime dispatcherFactoryLifetime)
        {
            services.AddService<ICommandDispatcher, CommandDispatcher>(dispatchersLifeTime);
            services.AddService<IEventDispatcher, EventDispatcher>(dispatchersLifeTime);
            services.AddService<IQueryDispatcher, QueryDispatcher>(dispatchersLifeTime); 
            services.AddService<IDomainDispatcher, DomainDispatcher>(dispatcherFactoryLifetime);
            return services;
        }
    }
}