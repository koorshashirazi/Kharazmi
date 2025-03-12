using System;
using Hangfire;
using Hangfire.Dashboard;
using Hangfire.MemoryStorage;
using Hangfire.Mongo;
using Hangfire.SqlServer;
using Kharazmi.AspNetCore.Core;
using Kharazmi.AspNetCore.Core.GuardToolkit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MongoDB.Driver;

namespace Kharazmi.BackgroundJob
{
    /// <summary> </summary>
    public static class CoreFrameworkBuilderExtensions
    {
        /// <summary> </summary>
        public static CoreFrameworkBuilder AddBackgroundService(this CoreFrameworkBuilder builder,
            JobStoreOptions jobStoreOptions)
        {
            builder.Services.AddScoped<IBackgroundJobClient>(sp => new BackgroundJobClient());
            builder.Services.TryAddScoped<IBackgroundService, BackgroundService>();
            builder.Services.TryAddSingleton(sp => jobStoreOptions);

            Ensure.ArgumentIsNotNull(jobStoreOptions, nameof(jobStoreOptions));

            switch (jobStoreOptions.JobStorageType)
            {
                case JobStorageType.InMemory:
                    builder.Services.AddHangfire(config => config
                        .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                        .UseSimpleAssemblyNameTypeSerializer()
                        .UseRecommendedSerializerSettings()
                        .UseSerilogLogProvider()
                        .UseFilter(new AutomaticRetryAttribute
                        {
                            Attempts = jobStoreOptions.Attempts,
                            DelaysInSeconds = jobStoreOptions.DelaysInSeconds,
                            OnAttemptsExceeded = AttemptsExceededAction.Fail
                        })
                        .UseFilter(new ProlongExpirationTimeAttribute())
                        .UseFilter(new JobExceptionFilterAttribute())
                        .UseMemoryStorage(new MemoryStorageOptions
                        {
                            JobExpirationCheckInterval = TimeSpan.FromSeconds(jobStoreOptions.JobExpirationFromSeceonds)
                        }));
                    break;
                case JobStorageType.Redis:
                    throw new NotSupportedException();
                case JobStorageType.MongoDb:
                    
                    var mongoOptions = jobStoreOptions.MongoDbStorageOptions;
                    var mongoUrlBuilder = new MongoUrlBuilder(mongoOptions.MongoDbConnection);
                    var mongoClient = new MongoClient(mongoUrlBuilder.ToMongoUrl());
                    builder.Services.AddHangfire(configuration => configuration
                        .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                        .UseSimpleAssemblyNameTypeSerializer()
                        .UseRecommendedSerializerSettings()
                        .UseSerilogLogProvider()
                        .UseMongoStorage(mongoClient, mongoUrlBuilder.DatabaseName,mongoOptions)
                    );
                    break;
                case JobStorageType.SqlServer:
                    builder.Services.AddHangfire(config =>
                    {
                        config
                            .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                            .UseSimpleAssemblyNameTypeSerializer()
                            .UseRecommendedSerializerSettings()
                            .UseSerilogLogProvider()
                            .UseFilter(new AutomaticRetryAttribute
                            {
                                Attempts = jobStoreOptions.Attempts,
                                DelaysInSeconds = jobStoreOptions.DelaysInSeconds,
                                OnAttemptsExceeded = AttemptsExceededAction.Fail
                            })
                            .UseFilter(new ProlongExpirationTimeAttribute())
                            .UseFilter(new JobExceptionFilterAttribute())
                            .UseSqlServerStorage(jobStoreOptions.SqlServerConnection,
                                new SqlServerStorageOptions
                                {
                                    SchemaName = jobStoreOptions.SchemaName,
                                    QueuePollInterval = TimeSpan.FromSeconds(jobStoreOptions.QueuePollIntervalFromSeconds),
                                    UseRecommendedIsolationLevel = jobStoreOptions.UseRecommendedIsolationLevel,
                                    JobExpirationCheckInterval = TimeSpan.FromSeconds(jobStoreOptions.JobExpirationFromSeceonds),
                                    UsePageLocksOnDequeue = jobStoreOptions.UsePageLocksOnDequeue,
                                    UseFineGrainedLocks = jobStoreOptions.UseFineGrainedLocks
                                });
                    });
                    break;
                default:
                    throw new NotSupportedException();
            }

            return builder;
        }

        /// <summary> </summary>
        public static IApplicationBuilder UseBackgroundTask(this IApplicationBuilder app)
        {
            var services = app.ApplicationServices;

            var dashboardOptions = services.GetService<DashboardOptions>() ?? new DashboardOptions();
            var storageOptions = services.GetRequiredService<JobStorage>();
            var routeOptions = services.GetRequiredService<RouteCollection>();
            var jobStoreOptions = services.GetRequiredService<JobStoreOptions>();

            if (!jobStoreOptions.Enable) return app;

            if (jobStoreOptions.EnableDashboard)
            {
                dashboardOptions.TimeZoneResolver ??= services.GetService<ITimeZoneResolver>();
                if (jobStoreOptions.UseAuthorizationFilter)
                {
                    dashboardOptions.Authorization = jobStoreOptions.AuthorizationFilters;
                }

                dashboardOptions.DashboardTitle = jobStoreOptions.DashboardTitle;


                app.Map(new PathString(jobStoreOptions.PathMatch),
                    builder =>
                        builder.UseMiddleware<JobStoreDashboardMiddleware>(storageOptions, dashboardOptions,
                            routeOptions,
                            jobStoreOptions));
            }

            app.UseHangfireServer(jobStoreOptions.ServerOptions);

            return app;
        }
    }
}