using System;
using System.Globalization;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace Kharazmi.AspNetCore.Localization.EFCore
{
    public class EfDistributedCacheStringLocalizer<TContext> : BaseStringLocalization
        where TContext : DbContext
    {
        private readonly IServiceProvider _resolver;
        private readonly IDistributedCache _cache;

        public EfDistributedCacheStringLocalizer(
            string resourceName,
            IServiceProvider resolver,
            IDistributedCache cache) : base("", resourceName)
        {
            _resolver = resolver ?? throw new ArgumentException(nameof(resolver));
            _cache = cache ?? throw new ArgumentException(nameof(cache));
        }


        protected override string GetComputedKey(string name)
        {
            var value = string.Empty;
            var computedKey = base.GetComputedKey(name);
            value = _cache.GetOrCreateExclusive(computedKey, () =>
            {
                return _resolver.RunScopedService<string, TContext>(context =>
                {
                    return value = context.Set<LocalizationEntity>()
                                       .FirstOrDefault(a =>
                                           a.Key == name && a.CultureName == CultureInfo.CurrentUICulture.Name)
                                       ?.Value ?? name;
                });
            });

            return value;
        }
    }
}