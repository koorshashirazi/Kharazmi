using System;
using System.Linq;
using Kharazmi.AspNetCore.Core.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Kharazmi.AspNetCore.Web.Configuration
{
    public static class AppSettingsExtensions
    {
        public static IWebHostBuilder ConfigureAppSettings<TAppSetting>(
            this IWebHostBuilder webHostBuilder,
            IConfigurationRoot configuration = null,
            Action<TAppSetting> appSetting = null) where TAppSetting : AppSetting
        {
            return webHostBuilder
                .ConfigureServices((ctx, services) =>
                {
                    configuration ??= new ConfigurationBuilder()
                        .SetBasePath(ctx.HostingEnvironment.ContentRootPath)
                        .Add(new AppSettingsConfigurationSource
                        {
                            Path = $"appsettings.{ctx.HostingEnvironment.EnvironmentName}.json"
                        })
                        .AddEnvironmentVariables()
                        .Build();


                    services.AddSingleton(_ => configuration);
                    services.Configure<TAppSetting>(configuration);

                    var configurationProvider = configuration.Providers.FirstOrDefault() as SettingsProvider;

                    services.AddSingleton(typeof(ISettingsService<TAppSetting>), sp =>
                        new SettingsService<TAppSetting>(configurationProvider,
                            sp.GetRequiredService<IOptionsMonitor<TAppSetting>>()));

                    var provider = services.BuildServiceProvider();
                    var settingsManager = provider.GetService<ISettingsService<TAppSetting>>();
                    if (settingsManager.Current == null) return;
                    if (settingsManager.Current.UpdateOnLoad)
                        settingsManager.Update(appSetting);
                });
        }


        public static IHostBuilder ConfigureAppSettings<TAppSetting>(
            this IHostBuilder webHostBuilder,
            IConfigurationRoot configuration = null,
            Action<TAppSetting> appSetting = null) where TAppSetting : AppSetting
        {
            return webHostBuilder
                .ConfigureServices((ctx, services) =>
                {
                    configuration ??= new ConfigurationBuilder()
                        .SetBasePath(ctx.HostingEnvironment.ContentRootPath)
                        .Add(new AppSettingsConfigurationSource
                        {
                            Path = $"appsettings.{ctx.HostingEnvironment.EnvironmentName}.json"
                        })
                        .AddEnvironmentVariables()
                        .Build();


                    services.AddSingleton(_ => configuration);
                    services.Configure<TAppSetting>(configuration);

                    var configurationProvider = configuration.Providers.FirstOrDefault() as SettingsProvider;

                    services.AddSingleton(typeof(ISettingsService<TAppSetting>), sp =>
                        new SettingsService<TAppSetting>(configurationProvider,
                            sp.GetRequiredService<IOptionsMonitor<TAppSetting>>()));

                    var provider = services.BuildServiceProvider();
                    var settingsManager = provider.GetService<ISettingsService<TAppSetting>>();
                    if (settingsManager.Current == null) return;
                    if (settingsManager.Current.UpdateOnLoad)
                        settingsManager.Update(appSetting);
                });
        }
    }
}