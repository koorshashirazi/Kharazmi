using System;
using Kharazmi.AspNetCore.Core.Dependency;
using Kharazmi.AspNetCore.Core.GuardToolkit;
using Microsoft.Extensions.DependencyInjection;

namespace Kharazmi.AspNetCore.Core.Tenancy
{
 /// <summary>
    /// Provides multi-tenancy service collection extensions
    /// </summary>
    public static class TenantServiceCollectionExtensions
    {
        /// <summary>
        /// Adds a tenant-scoped service
        /// </summary>
        /// <param name="services">The service collection</param>
        /// <param name="serviceType">The service type</param>
        /// <returns>The service collection</returns>
        public static IServiceCollection AddTenantScoped(this IServiceCollection services, Type serviceType)
        {
            
            Ensure.ArgumentIsNotNull(services, nameof(services));

            services.Add(new TenantScopedServiceDescriptor(
                Ensure.ArgumentIsNotNull(serviceType, nameof(serviceType)),
                serviceType));

            return services;
        }

        /// <summary>
        /// Adds a tenant-scoped service
        /// </summary>
        /// <param name="services">The service collection</param>
        /// <param name="serviceType">The service type</param>
        /// <param name="implementationType">The implementation type</param>
        /// <returns>The service collection</returns>
        public static IServiceCollection AddTenantScoped(this IServiceCollection services, Type serviceType, Type implementationType)
        {
            Ensure.ArgumentIsNotNull(services, nameof(services));

            services.Add(new TenantScopedServiceDescriptor(
                Ensure.ArgumentIsNotNull(serviceType, nameof(serviceType)),
                Ensure.ArgumentIsNotNull(implementationType, nameof(implementationType))));

            return services;
        }

        /// <summary>
        /// Adds a tenant-scoped service
        /// </summary>
        /// <param name="services">The service collection</param>
        /// <param name="serviceType">The service type</param>
        /// <param name="factory">The service instance factory</param>
        /// <returns>The service collection</returns>
        public static IServiceCollection AddTenantScoped(this IServiceCollection services, Type serviceType, Func<IServiceProvider, object> factory)
        {
            Ensure.ArgumentIsNotNull(services, nameof(services));

            services.Add(new TenantScopedServiceDescriptor(
                Ensure.ArgumentIsNotNull(serviceType, nameof(serviceType)),
                Ensure.ArgumentIsNotNull(factory, nameof(factory))));

            return services;
        }

        /// <summary>
        /// Adds a tenant-scoped service
        /// </summary>
        /// <param name="services">The service collection</param>
        /// <typeparam name="TService">The service type</typeparam>
        /// <returns>The service collection</returns>
        public static IServiceCollection AddTenantScoped<TService>(this IServiceCollection services)
            where TService : class
        {
            Ensure.ArgumentIsNotNull(services, nameof(services));

            services.Add(new TenantScopedServiceDescriptor(
                typeof(TService),
                typeof(TService)));

            return services;
        }

        /// <summary>
        /// Adds a tenant-scoped service
        /// </summary>
        /// <param name="services">The service collection</param>
        /// <typeparam name="TService">The service type</typeparam>
        /// <typeparam name="TImplementation">The implementation tyoe</typeparam>
        /// <returns>The service collection</returns>
        public static IServiceCollection AddTenantScoped<TService, TImplementation>(this IServiceCollection services)
            where TService : class
            where TImplementation : TService
        {
            Ensure.ArgumentIsNotNull(services, nameof(services));

            services.Add(new TenantScopedServiceDescriptor(
                typeof(TService),
                typeof(TImplementation)));

            return services;
        }

        /// <summary>
        /// Adds a tenant-scoped service
        /// </summary>
        /// <param name="services">The service collection</param>
        /// <typeparam name="TService">The service type</typeparam>
        /// <param name="factory">The service instance factory</param>
        /// <returns>The service collection</returns>
        public static IServiceCollection AddTenantScoped<TService>(this IServiceCollection services, Func<IServiceProvider, TService> factory)
            where TService : class
        {
            Ensure.ArgumentIsNotNull(services, nameof(services));

            services.Add(new TenantScopedServiceDescriptor(
                typeof(TService),
                Ensure.ArgumentIsNotNull(factory, nameof(factory))));


            return services;
        }

        /// <summary>
        /// Adds a tenant-scoped service
        /// </summary>
        /// <param name="services">The service collection</param>
        /// <typeparam name="TService">The service type</typeparam>
        /// <typeparam name="TImplementation"></typeparam>
        /// <param name="factory">The service instance factory</param>
        /// <returns>The service collection</returns>
        public static IServiceCollection AddTenantScoped<TService, TImplementation>(this IServiceCollection services, Func<IServiceProvider, TImplementation> factory)
            where TService : class
            where TImplementation : TService
        {
            Ensure.ArgumentIsNotNull(services, nameof(services));
            Ensure.ArgumentIsNotNull(factory, nameof(factory));

            services.Add(new TenantScopedServiceDescriptor(
                typeof(TService),
                sp => factory(sp)));


            return services;
        }
    }
}