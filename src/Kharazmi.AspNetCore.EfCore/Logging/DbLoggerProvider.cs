using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Kharazmi.AspNetCore.Core.Extensions;
using Kharazmi.AspNetCore.Core.Logging;
using Kharazmi.AspNetCore.EFCore.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

// ReSharper disable InconsistentNaming

namespace Kharazmi.AspNetCore.EFCore.Logging
{
    public static class LoggerFactoryExtensions
    {
        public static ILoggingBuilder AddDbLogger<TContext>(this ILoggingBuilder builder)
            where TContext : DbContext, IUnitOfWork<TContext>
        {
            builder.Services.AddSingleton<ILoggerProvider, DbLoggerProvider<TContext>>();
            return builder;
        }

        public static ILoggingBuilder AddDbLogger<TContext>(this ILoggingBuilder builder, Action<DbLoggerOptions>
            configuration)
            where TContext : DbContext, IUnitOfWork<TContext>
        {
            builder.AddDbLogger<TContext>();
            builder.Services.Configure(configuration);

            return builder;
        }
    }

    [ProviderAlias("EFCore")]
    internal class DbLoggerProvider<TContext> : BatchingLoggerProvider
        where TContext : DbContext, IUnitOfWork<TContext>
    {
        private readonly IServiceProvider _provider;

        public DbLoggerProvider(IOptions<DbLoggerOptions> options, IServiceProvider provider) : base(options, provider)
        {
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
        }

        protected override async Task WriteMessagesAsync(IEnumerable<LogMessage> messages, CancellationToken token)
        {
            var logs = messages.Where(m => m.Message.IsNotEmpty())
                .Select(m => new Log
                {
                    Level = m.Level.ToString(),
                    LoggerName = m.LoggerName,
                    EventId = m.EventId.Id,
                    Message = m.Message,
                    UserIP = m.UserIP,
                    UserId = m.UserId,
                    UserBrowserName = m.UserBrowserName,
                    UserDisplayName = m.UserDisplayName,
                    UserName = m.UserName,
                    CreationTime = m.CreationTime
                });

            using var scope = _provider.CreateScope();
            using var context = scope.ServiceProvider.GetRequiredService<IUnitOfWork<TContext>>();
            await context.Set<Log>().AddRangeAsync(logs, token).ConfigureAwait(false);
            await context.SaveChangesAsync(token).ConfigureAwait(false);
        }
    }
}