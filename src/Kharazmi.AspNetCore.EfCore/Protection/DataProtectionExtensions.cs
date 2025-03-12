using Kharazmi.AspNetCore.Core.Cryptography;
using Kharazmi.AspNetCore.EFCore.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Kharazmi.AspNetCore.EFCore.Protection
{
    public interface IDataProtectionBuilder
    {
        IServiceCollection Services { get; }
    }

    public class DataProtectionBuilder : IDataProtectionBuilder
    {
        public DataProtectionBuilder(IServiceCollection services)
        {
            Services = services;
        }

        public IServiceCollection Services { get; }
    }

    public static class DataProtectionExtensions
    {
        public static IDataProtectionBuilder PersistKeysToDbContext<TContext>(this IServiceCollection services)
            where TContext : DbContext, IUnitOfWork<TContext>
        {
            services.TryAddSingleton<IProtectionStore, ProtectionStore<TContext>>();
            return new DataProtectionBuilder(services);
        }
    }
}