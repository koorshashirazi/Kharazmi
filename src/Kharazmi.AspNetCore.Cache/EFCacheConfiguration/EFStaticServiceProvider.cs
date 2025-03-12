using System;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;

namespace Kharazmi.AspNetCore.Cache.EFCacheConfiguration
{
    /// <summary>
    /// A lazy loaded thread-safe singleton App ServiceProvider.
    /// It's required for static `Cacheable()` methods.
    /// </summary>
    public static class EFStaticServiceProvider
    {
        private static readonly Lazy<IServiceProvider> _serviceProviderBuilder =
            new Lazy<IServiceProvider>(GetServiceProvider, LazyThreadSafetyMode.ExecutionAndPublication);

        /// <summary>
        /// Defines a mechanism for retrieving a service object.
        /// </summary>
        public static IServiceProvider Instance { get; } = _serviceProviderBuilder.Value;

        private static IServiceProvider GetServiceProvider()
        {
            var serviceProvider = EFServiceCollectionExtensions.ServiceCollection?.BuildServiceProvider();
            return serviceProvider ?? throw new InvalidOperationException("Please add `AddEFSecondLevelCache()` method to your `IServiceCollection`.");
        }
    }
}