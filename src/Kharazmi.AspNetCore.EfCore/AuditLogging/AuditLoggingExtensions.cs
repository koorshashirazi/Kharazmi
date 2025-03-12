using System;
using System.Collections.Generic;
using Kharazmi.AspNetCore.Core.AuditLogging;
using Kharazmi.AspNetCore.EFCore.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Kharazmi.AspNetCore.EFCore.AuditLogging
{
    public static class AuditLoggingExtensions
    {
        public static IAuditLoggingBuilder AddAuditLoggingBuilder(
            this IServiceCollection services)
        {
            return new AuditLoggingBuilder(services);
        }

        public static IAuditLoggingBuilder AddAuditLogging<TAuditLoggerOptions>(
            this IServiceCollection service,
            Action<TAuditLoggerOptions> loggerOptions = null) where TAuditLoggerOptions : AuditLoggerOptions, new()
        {
            var auditLoggingBuilder = service.AddAuditLoggingBuilder();

            var implementationInstance = new TAuditLoggerOptions();
            loggerOptions?.Invoke(implementationInstance);
            auditLoggingBuilder.Services.AddSingleton(implementationInstance);

            auditLoggingBuilder.Services.AddTransient<IAuditEventLogger, AuditEventLogger>();
            return auditLoggingBuilder;
        }

        public static IAuditLoggingBuilder AddAuditLogging(
            this IServiceCollection service,
            Action<AuditLoggerOptions> loggerOptions = null)
        {
            return service.AddAuditLogging<AuditLoggerOptions>(loggerOptions);
        }

        public static IAuditLoggingBuilder WithDefaultRepository(
            this IAuditLoggingBuilder builder)
        {
            builder.WithRepository<DefaultAuditLoggingDbContext, AuditLog>();
            return builder;
        }

        public static IAuditLoggingBuilder WithRepository<TDbContext, TAuditLog>(this IAuditLoggingBuilder builder)
            where TDbContext : DbContext, IUnitOfWork<TDbContext>, IAuditLoggingDbContext<TAuditLog>
            where TAuditLog : AuditLog
        {
            builder.Services
                .TryAddTransient<IAuditLoggingRepository<TDbContext, TAuditLog>,
                    AuditLoggingRepository<TDbContext, TAuditLog>>();
            return builder;
        }


        public static IAuditLoggingBuilder WithStaticEventSubject(
            this IAuditLoggingBuilder builder,
            Action<DefaultAuditSubject> defaultAuditSubject)
        {
            DefaultAuditSubject defaultAuditSubject1 = new DefaultAuditSubject();
            defaultAuditSubject?.Invoke(defaultAuditSubject1);
            builder.Services.AddSingleton((IAuditSubject) defaultAuditSubject1);
            return builder;
        }

        public static IAuditLoggingBuilder WithEventSubject(
            this IAuditLoggingBuilder builder)
        {
            builder.Services.TryAddTransient<IAuditSubject, DefaultAuditSubject>();
            return builder;
        }

        public static IAuditLoggingBuilder WithEventAction(
            this IAuditLoggingBuilder builder)
        {
            builder.Services.TryAddTransient<IAuditAction, DefaultAuditAction>();
            return builder;
        }

        public static IAuditLoggingBuilder WithEventData(
            this IAuditLoggingBuilder builder)
        {
            builder.Services.TryAddTransient<IAuditSubject, DefaultAuditSubject>();
            builder.Services.TryAddTransient<IAuditAction, DefaultAuditAction>();
            return builder;
        }

        public static IAuditLoggingBuilder WithEventData<TEventSubject, TEventAction>(
            this IAuditLoggingBuilder builder)
            where TEventSubject : class, IAuditSubject
            where TEventAction : class, IAuditAction
        {
            builder.Services.AddTransient<IAuditSubject, TEventSubject>();
            builder.Services.AddTransient<IAuditAction, TEventAction>();
            return builder;
        }

        public static IAuditLoggingBuilder AddAuditSinks<T1>(
            this IAuditLoggingBuilder builder)
            where T1 : class, IAuditEventLoggerSink
        {
            builder.Services.TryAddEnumerable(new ServiceDescriptor[1]
            {
                ServiceDescriptor.Transient<IAuditEventLoggerSink, T1>()
            });
            return builder;
        }

        public static IAuditLoggingBuilder AddAuditSinks<T1, T2>(
            this IAuditLoggingBuilder builder)
            where T1 : class, IAuditEventLoggerSink
            where T2 : class, IAuditEventLoggerSink
        {
            builder.Services.TryAddEnumerable((IEnumerable<ServiceDescriptor>) new ServiceDescriptor[2]
            {
                ServiceDescriptor.Transient<IAuditEventLoggerSink, T1>(),
                ServiceDescriptor.Transient<IAuditEventLoggerSink, T2>()
            });
            return builder;
        }

        public static IAuditLoggingBuilder AddAuditSinks<T1, T2, T3>(
            this IAuditLoggingBuilder builder)
            where T1 : class, IAuditEventLoggerSink
            where T2 : class, IAuditEventLoggerSink
            where T3 : class, IAuditEventLoggerSink
        {
            builder.Services.TryAddEnumerable((IEnumerable<ServiceDescriptor>) new ServiceDescriptor[3]
            {
                ServiceDescriptor.Transient<IAuditEventLoggerSink, T1>(),
                ServiceDescriptor.Transient<IAuditEventLoggerSink, T2>(),
                ServiceDescriptor.Transient<IAuditEventLoggerSink, T3>()
            });
            return builder;
        }

        public static IAuditLoggingBuilder AddAuditSinks<T1, T2, T3, T4>(
            this IAuditLoggingBuilder builder)
            where T1 : class, IAuditEventLoggerSink
            where T2 : class, IAuditEventLoggerSink
            where T3 : class, IAuditEventLoggerSink
            where T4 : class, IAuditEventLoggerSink
        {
            builder.Services.TryAddEnumerable((IEnumerable<ServiceDescriptor>) new ServiceDescriptor[4]
            {
                ServiceDescriptor.Transient<IAuditEventLoggerSink, T1>(),
                ServiceDescriptor.Transient<IAuditEventLoggerSink, T2>(),
                ServiceDescriptor.Transient<IAuditEventLoggerSink, T3>(),
                ServiceDescriptor.Transient<IAuditEventLoggerSink, T4>()
            });
            return builder;
        }

        public static IAuditLoggingBuilder AddAuditSinks<T1, T2, T3, T4, T5>(
            this IAuditLoggingBuilder builder)
            where T1 : class, IAuditEventLoggerSink
            where T2 : class, IAuditEventLoggerSink
            where T3 : class, IAuditEventLoggerSink
            where T4 : class, IAuditEventLoggerSink
            where T5 : class, IAuditEventLoggerSink
        {
            builder.Services.TryAddEnumerable((IEnumerable<ServiceDescriptor>) new ServiceDescriptor[5]
            {
                ServiceDescriptor.Transient<IAuditEventLoggerSink, T1>(),
                ServiceDescriptor.Transient<IAuditEventLoggerSink, T2>(),
                ServiceDescriptor.Transient<IAuditEventLoggerSink, T3>(),
                ServiceDescriptor.Transient<IAuditEventLoggerSink, T4>(),
                ServiceDescriptor.Transient<IAuditEventLoggerSink, T5>()
            });
            return builder;
        }

        public static IAuditLoggingBuilder AddAuditSinks<T1, T2, T3, T4, T5, T6>(
            this IAuditLoggingBuilder builder)
            where T1 : class, IAuditEventLoggerSink
            where T2 : class, IAuditEventLoggerSink
            where T3 : class, IAuditEventLoggerSink
            where T4 : class, IAuditEventLoggerSink
            where T5 : class, IAuditEventLoggerSink
            where T6 : class, IAuditEventLoggerSink
        {
            builder.Services.TryAddEnumerable((IEnumerable<ServiceDescriptor>) new ServiceDescriptor[6]
            {
                ServiceDescriptor.Transient<IAuditEventLoggerSink, T1>(),
                ServiceDescriptor.Transient<IAuditEventLoggerSink, T2>(),
                ServiceDescriptor.Transient<IAuditEventLoggerSink, T3>(),
                ServiceDescriptor.Transient<IAuditEventLoggerSink, T4>(),
                ServiceDescriptor.Transient<IAuditEventLoggerSink, T5>(),
                ServiceDescriptor.Transient<IAuditEventLoggerSink, T6>()
            });
            return builder;
        }

        public static IAuditLoggingBuilder AddAuditSinks<T1, T2, T3, T4, T5, T6, T7>(
            this IAuditLoggingBuilder builder)
            where T1 : class, IAuditEventLoggerSink
            where T2 : class, IAuditEventLoggerSink
            where T3 : class, IAuditEventLoggerSink
            where T4 : class, IAuditEventLoggerSink
            where T5 : class, IAuditEventLoggerSink
            where T6 : class, IAuditEventLoggerSink
            where T7 : class, IAuditEventLoggerSink
        {
            builder.Services.TryAddEnumerable((IEnumerable<ServiceDescriptor>) new ServiceDescriptor[7]
            {
                ServiceDescriptor.Transient<IAuditEventLoggerSink, T1>(),
                ServiceDescriptor.Transient<IAuditEventLoggerSink, T2>(),
                ServiceDescriptor.Transient<IAuditEventLoggerSink, T3>(),
                ServiceDescriptor.Transient<IAuditEventLoggerSink, T4>(),
                ServiceDescriptor.Transient<IAuditEventLoggerSink, T5>(),
                ServiceDescriptor.Transient<IAuditEventLoggerSink, T6>(),
                ServiceDescriptor.Transient<IAuditEventLoggerSink, T7>()
            });
            return builder;
        }

        public static IAuditLoggingBuilder AddAuditSinks<T1, T2, T3, T4, T5, T6, T7, T8>(
            this IAuditLoggingBuilder builder)
            where T1 : class, IAuditEventLoggerSink
            where T2 : class, IAuditEventLoggerSink
            where T3 : class, IAuditEventLoggerSink
            where T4 : class, IAuditEventLoggerSink
            where T5 : class, IAuditEventLoggerSink
            where T6 : class, IAuditEventLoggerSink
            where T7 : class, IAuditEventLoggerSink
            where T8 : class, IAuditEventLoggerSink
        {
            builder.Services.TryAddEnumerable((IEnumerable<ServiceDescriptor>) new ServiceDescriptor[8]
            {
                ServiceDescriptor.Transient<IAuditEventLoggerSink, T1>(),
                ServiceDescriptor.Transient<IAuditEventLoggerSink, T2>(),
                ServiceDescriptor.Transient<IAuditEventLoggerSink, T3>(),
                ServiceDescriptor.Transient<IAuditEventLoggerSink, T4>(),
                ServiceDescriptor.Transient<IAuditEventLoggerSink, T5>(),
                ServiceDescriptor.Transient<IAuditEventLoggerSink, T6>(),
                ServiceDescriptor.Transient<IAuditEventLoggerSink, T7>(),
                ServiceDescriptor.Transient<IAuditEventLoggerSink, T8>()
            });
            return builder;
        }
    }
}