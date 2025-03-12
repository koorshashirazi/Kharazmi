using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Scrutor;

namespace Kharazmi.AspNetCore.Core.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        /// <param name="serviceType"></param>
        /// <param name="implementationsType"></param>
        /// <param name="serviceLifetime"></param>
        public static void AddService(this IServiceCollection services,
            Type serviceType,
            Type implementationsType,
            ServiceLifetime serviceLifetime)
        {
            services.Add(new ServiceDescriptor(serviceType, implementationsType, serviceLifetime));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        /// <param name="implementationFactory"></param>
        /// <param name="serviceLifetime"></param>
        /// <typeparam name="TService"></typeparam>
        public static void AddService<TService>(this IServiceCollection services,
            Func<IServiceProvider, TService> implementationFactory,
            ServiceLifetime serviceLifetime)
            where TService : class
        {
            services.Add(new ServiceDescriptor(typeof(TService), implementationFactory, serviceLifetime));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        /// <param name="serviceType"></param>
        /// <param name="implementationsType"></param>
        /// <param name="serviceLifetime"></param>
        public static void TryAddService(this IServiceCollection services,
            Type serviceType,
            Type implementationsType,
            ServiceLifetime serviceLifetime)
        {
            services.TryAdd(new ServiceDescriptor(serviceType, implementationsType, serviceLifetime));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        /// <param name="implementationFactory"></param>
        /// <param name="serviceLifetime"></param>
        /// <typeparam name="TResponse"></typeparam>
        /// <typeparam name="TRequest"></typeparam>
        public static void TryAddService<TResponse, TRequest>(this IServiceCollection services,
            Func<IServiceProvider, TRequest> implementationFactory,
            ServiceLifetime serviceLifetime)
            where TRequest : class, TResponse
        {
            services.TryAdd(new ServiceDescriptor(typeof(TResponse), implementationFactory, serviceLifetime));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        /// <param name="serviceLifetime"></param>
        /// <typeparam name="TRequest"></typeparam>
        /// <typeparam name="TResponse"></typeparam>
        public static void AddService<TResponse, TRequest>(this IServiceCollection services,
            ServiceLifetime serviceLifetime)
            where TRequest : class, TResponse
        {
            services.Add(new ServiceDescriptor(typeof(TResponse), typeof(TRequest), serviceLifetime));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        /// <param name="serviceLifetime"></param>
        /// <typeparam name="TRequest"></typeparam>
        /// <typeparam name="TResponse"></typeparam>
        public static void TryAddService<TResponse, TRequest>(this IServiceCollection services,
            ServiceLifetime serviceLifetime)
            where TRequest : class, TResponse
        {
            services.TryAdd(new ServiceDescriptor(typeof(TResponse), typeof(TRequest), serviceLifetime));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        /// <param name="type"></param>
        /// <param name="serviceLifetime"></param>
        /// <param name="assemblies"></param>
        /// <typeparam name="T"></typeparam>
        public static IServiceCollection RegisterTypeFromAssembly(this IServiceCollection services, Type type,
            ServiceLifetime serviceLifetime = ServiceLifetime.Transient, Assembly[] assemblies = null)
        {
            assemblies ??= new[] {Assembly.GetCallingAssembly()};
            services.Scan(scan => scan
                .FromAssemblies(assemblies)
                .AddClasses(classes => classes.AssignableTo(type))
                .UsingRegistrationStrategy(RegistrationStrategy.Skip)
                .AsImplementedInterfaces()
                .WithLifetime(serviceLifetime));

            return services;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <param name="typeName"></param>
        /// <typeparam name="TInterface"></typeparam>
        /// <returns></returns>
        /// <exception cref="TypeAccessException"></exception>
        public static TInterface ResolveByName<TInterface>(this IServiceProvider serviceProvider, string typeName)
        {
            var allRegisteredTypes = serviceProvider.GetRequiredService<IEnumerable<TInterface>>();
            var resolvedService = allRegisteredTypes.FirstOrDefault(p =>
            {
                var fullName = p.GetType().FullName;
                return fullName != null && fullName.Contains(typeName);
            });

            if (resolvedService != null)
            {
                return resolvedService;
            }

            throw new TypeAccessException($"{typeName} type not found.");
        }
    }
}