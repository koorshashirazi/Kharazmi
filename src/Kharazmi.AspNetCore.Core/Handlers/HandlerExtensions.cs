using System;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Scrutor;

namespace Kharazmi.AspNetCore.Core.Handlers
{
    internal static class HandlerExtensions
    {
        internal static IServiceCollection RegisterHandlers(this IServiceCollection services,
            ServiceLifetime serviceLifetime = ServiceLifetime.Transient, Assembly[]? assemblies = null)
        {
            var assemblyList = assemblies ?? new[] { Assembly.GetEntryAssembly() };
            services.Scan(types => types.FromAssemblies(assemblyList)
                .AddClasses(c =>
                {
                    c.AssignableTo(typeof(IDomainEventHandler<>));
                    c.Where(t =>
                        !t.IsAbstract && t.IsClass && t.Name.EndsWith("Handler", StringComparison.OrdinalIgnoreCase));
                })
                .UsingRegistrationStrategy(RegistrationStrategy.Replace(ReplacementBehavior.ImplementationType))
                .AsImplementedInterfaces()
                .WithLifetime(serviceLifetime)
                .AddClasses(c =>
                {
                    c.AssignableTo(typeof(IDomainCommandHandler<>));
                    c.Where(t =>
                        !t.IsAbstract && t.IsClass && t.Name.EndsWith("Handler", StringComparison.OrdinalIgnoreCase));
                })
                .UsingRegistrationStrategy(RegistrationStrategy.Replace(ReplacementBehavior.ImplementationType))
                .AsImplementedInterfaces()
                .WithLifetime(serviceLifetime)
                .AddClasses(c =>
                {
                    c.AssignableTo(typeof(IDomainQueryHandler<,>));
                    c.Where(t =>
                        !t.IsAbstract && t.IsClass && t.Name.EndsWith("Handler", StringComparison.OrdinalIgnoreCase));
                })
                .UsingRegistrationStrategy(RegistrationStrategy.Replace(ReplacementBehavior.ImplementationType))
                .AsImplementedInterfaces()
                .WithLifetime(serviceLifetime)
            );

            return services;
        }
    }
}