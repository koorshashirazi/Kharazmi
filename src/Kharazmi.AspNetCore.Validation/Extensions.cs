using System;
using System.Collections.Generic;
using System.Reflection;
using FluentValidation;
using Kharazmi.AspNetCore.Core.Bus;
using Kharazmi.AspNetCore.Core.Extensions;
using Kharazmi.AspNetCore.Core.Handlers;
using Kharazmi.AspNetCore.Core.Pipelines;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Kharazmi.AspNetCore.Validation
{
    public class ValidationBuilder
    {
        public ValidationBuilder(IServiceCollection services)
        {
            Services = services;
        }

        public IServiceCollection Services { get; }
    }

    public static class Extensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="validationBuilder"></param>
        /// <param name="serviceLifetime"></param>
        /// <returns></returns>
        public static BusBuilder WithCommandValidation(
            this BusBuilder builder, ValidationBuilder validationBuilder,
            ServiceLifetime serviceLifetime = ServiceLifetime.Transient)
        {
            validationBuilder.WithFluentValidation();

            builder.Services
                .AddService(typeof(ICommandValidationHandler<>),
                    typeof(CommandValidationHandler<>), serviceLifetime);
            return builder;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="validationBuilder"></param>
        /// <returns></returns>
        public static PipelineBuilder WithValidationCommandPipeline(this PipelineBuilder builder,
            ValidationBuilder validationBuilder)
        {
            validationBuilder.WithFluentValidation();
            builder.Services
                .Decorate(typeof(ICommandHandler<>), typeof(ValidationCommandPipeline<>));
            return builder;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static ValidationBuilder WithFluentValidation(this ValidationBuilder builder)
        {
            builder.Services.TryAddTransient(typeof(Core.Validation.IValidator<>), typeof(FluentValidationValidator<>));
            builder.Services.TryAddTransient<IValidatorFactory, ServiceProviderValidatorFactory>();
            return builder;
        }

        /// <summary>
        /// Adds all validators in specified assemblies
        /// </summary>
        /// <param name="services">The collection of services</param>
        /// <param name="assemblies"></param>
        /// <param name="lifetime">The lifetime of the validators. The default is transient</param>
        /// <returns></returns>
        public static ValidationBuilder AddValidatorsFromAssemblies(this IServiceCollection services,
            IEnumerable<Assembly> assemblies, ServiceLifetime lifetime = ServiceLifetime.Transient)
        {
            foreach (var assembly in assemblies)
                services.AddValidatorsFromAssembly(assembly, lifetime);

            return new ValidationBuilder(services);
        }

        /// <summary>
        /// Adds all validators in specified assembly
        /// </summary>
        /// <param name="services">The collection of services</param>
        /// <param name="assembly">The assembly to scan</param>
        /// <param name="lifetime">The lifetime of the validators. The default is transient</param>
        /// <returns></returns>
        public static ValidationBuilder AddValidatorsFromAssembly(this IServiceCollection services,
            Assembly assembly = null,
            ServiceLifetime lifetime = ServiceLifetime.Transient)
        {
            if (assembly == null)
                assembly = Assembly.GetEntryAssembly();

            AssemblyScanner
                .FindValidatorsInAssembly(assembly)
                .ForEach(scanResult => services.AddScanResult(scanResult, lifetime));

            return new ValidationBuilder(services);
        }

        /// <summary>
        /// Adds all validators in the assembly of the specified type
        /// </summary>
        /// <param name="services">The collection of services</param>
        /// <param name="type">The type whose assembly to scan</param>
        /// <param name="lifetime">The lifetime of the validators. The default is transient</param>
        /// <returns></returns>
        public static ValidationBuilder AddValidatorsFromAssemblyContaining(this IServiceCollection services,
            Type type, ServiceLifetime lifetime = ServiceLifetime.Transient)
            => services.AddValidatorsFromAssembly(type.Assembly, lifetime);

        /// <summary>
        /// Adds all validators in the assembly of the type specified by the generic parameter
        /// </summary>
        /// <param name="services">The collection of services</param>
        /// <param name="lifetime">The lifetime of the validators. The default is transient</param>
        /// <returns></returns>
        public static ValidationBuilder AddValidatorsFromAssemblyContaining<T>(this IServiceCollection services,
            ServiceLifetime lifetime = ServiceLifetime.Transient)
            => services.AddValidatorsFromAssembly(typeof(T).Assembly, lifetime);

        /// <summary>
        /// Helper method to register a validator from an AssemblyScanner result
        /// </summary>
        /// <param name="services">The collection of services</param>
        /// <param name="scanResult">The scan result</param>
        /// <param name="lifetime">The lifetime of the validators. The default is transient</param>
        /// <returns></returns>
        private static IServiceCollection AddScanResult(this IServiceCollection services,
            AssemblyScanner.AssemblyScanResult scanResult, ServiceLifetime lifetime)
        {
            //Register as interface
            services.Add(
                new ServiceDescriptor(
                    scanResult.InterfaceType,
                    scanResult.ValidatorType,
                    lifetime));

            //Register as self
            services.Add(
                new ServiceDescriptor(
                    scanResult.ValidatorType,
                    scanResult.ValidatorType,
                    lifetime));

            return services;
        }
    }
}