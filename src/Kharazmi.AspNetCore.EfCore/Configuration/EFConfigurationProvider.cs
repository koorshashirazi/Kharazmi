using System;
using System.Linq;
using Kharazmi.AspNetCore.Core.Configuration;
using Kharazmi.AspNetCore.Core.Dependency;
using Kharazmi.AspNetCore.EFCore.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Kharazmi.AspNetCore.EFCore.Configuration
{
    // ReSharper disable once InconsistentNaming
    public class EFConfigurationProvider : ConfigurationProvider
    {
        private readonly IServiceProvider _provider;

        public EFConfigurationProvider(IServiceProvider provider)
        {
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
        }

        public override void Load()
        {
            _provider.RunScoped<IUnitOfWork>(uow =>
            {
                Data?.Clear();
                Data = uow.Set<KeyValue>()
                    .AsNoTracking()
                    .ToDictionary(c => c.Key, c => c.Value);
            });
        }
    }
}