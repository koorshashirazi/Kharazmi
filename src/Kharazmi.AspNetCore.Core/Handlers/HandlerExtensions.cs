using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Scrutor;

namespace Kharazmi.AspNetCore.Core.Handlers
{
    internal static class HandlerExtensions
    {
        internal static IServiceCollection RegisterHandlers(this IServiceCollection services,
            ServiceLifetime serviceLifetime = ServiceLifetime.Transient, Assembly[] assemblies = null)
        {
            assemblies ??= new[] {Assembly.GetEntryAssembly()};
            services.Scan(types => types.FromAssemblies(assemblies)
                .AddClasses(c =>
                {
                    c.AssignableTo(typeof(IEventHandler<>));
                    c.Where(t => !t.IsAbstract && t.IsClass && t.Name.EndsWith("Handler"));
                })
                .UsingRegistrationStrategy(RegistrationStrategy.Replace(ReplacementBehavior.ImplementationType))
                .AsImplementedInterfaces()
                .WithLifetime(serviceLifetime)
                .AddClasses(c =>
                {
                    c.AssignableTo(typeof(ICommandHandler<>));
                    c.Where(t => !t.IsAbstract && t.IsClass && t.Name.EndsWith("Handler"));
                })
                .UsingRegistrationStrategy(RegistrationStrategy.Replace(ReplacementBehavior.ImplementationType))
                .AsImplementedInterfaces()
                .WithLifetime(serviceLifetime)
                .AddClasses(c =>
                {
                    c.AssignableTo(typeof(IQueryHandler<,>));
                    c.Where(t => !t.IsAbstract && t.IsClass && t.Name.EndsWith("Handler"));
                })
                .UsingRegistrationStrategy(RegistrationStrategy.Replace(ReplacementBehavior.ImplementationType))
                .AsImplementedInterfaces()
                .WithLifetime(serviceLifetime)
            );

            return services;
        }
    }
}