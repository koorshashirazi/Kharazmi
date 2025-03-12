using Kharazmi.AspNetCore.Core.Configuration;
using Kharazmi.AspNetCore.Core.Extensions;
using Kharazmi.AspNetCore.EFCore.Configuration;
using Kharazmi.AspNetCore.EFCore.Context;
using Kharazmi.AspNetCore.EFCore.Context.Hooks;
using Kharazmi.AspNetCore.EFCore.Transaction;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Kharazmi.AspNetCore.EFCore
{
    /// <summary>
    ///     Nice method to create the EFCore builder
    /// </summary>
    public static class EfoCoreBuilderExtensions
    {
        /// <summary>
        ///  Add the services (application specific tenant class)
        /// where TDbContext : DbContext, IUnitOfWork<TDbContext/>
        /// </summary>
        /// <param name="services"></param>
        /// <param name="serviceLifetime"></param>
        /// <returns></returns>
        public static EFCoreBuilder AddEFCore<TDbContext>(this IServiceCollection services,
            ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
            where TDbContext : DbContext, IUnitOfWork<TDbContext>
        {
            services.TryAddService<IUnitOfWork<TDbContext>, TDbContext>(serviceLifetime);
            services.TryAddTransient<TransactionInterceptor<TDbContext>>();
            services.TryAddScoped<IKeyValueService, KeyValueService<TDbContext>>();
            services.TryAddTransient<IHook, PreUpdateRowVersionHook>();

            return new EFCoreBuilder(services, typeof(TDbContext));
        }
    }
}