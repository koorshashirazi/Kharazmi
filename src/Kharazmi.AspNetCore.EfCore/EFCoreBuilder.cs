using System;
using Kharazmi.AspNetCore.Core.Transaction;
using Kharazmi.AspNetCore.EFCore.Context;
using Kharazmi.AspNetCore.EFCore.Context.Hooks;
using Microsoft.Extensions.DependencyInjection;

// ReSharper disable InconsistentNaming

namespace Kharazmi.AspNetCore.EFCore
{
    public class EFCoreBuilder
    {
        public EFCoreBuilder(IServiceCollection services, Type contextType)
        {
            Services = services;
            ContextType = contextType;
        }

        public IServiceCollection Services { get; }
        public Type ContextType { get; }


        public EFCoreBuilder WithTransactionOptions(Action<TransactionOptions> options)
        {
            if (options != null)
                Services.Configure(options);
            return this;
        }

        public EFCoreBuilder WithRowLevelSecurityHook<TUserId>() where TUserId : IEquatable<TUserId>
        {
            Services.AddTransient<IHook, PreInsertRowLevelSecurityHook<TUserId>>();
            return this;
        }

        public EFCoreBuilder WithTrackingHook<TUserId>() where TUserId : IEquatable<TUserId>
        {
            Services.AddTransient<IHook, PreInsertCreationTrackingHook<TUserId>>();
            Services.AddTransient<IHook, PreUpdateModificationTrackingHook<TUserId>>();
            return this;
        }

        public EFCoreBuilder WithTenancyHook<TTenantId>() where TTenantId : IEquatable<TTenantId>
        {
            Services.AddTransient<IHook, PreInsertTenantEntityHook<TTenantId>>();
            return this;
        }

        public EFCoreBuilder WithRowIntegrityHook()
        {
            Services.AddTransient<IHook, RowIntegrityHook>();
            return this;
        }

        public EFCoreBuilder WithDeletedEntityHook()
        {
            Services.AddTransient<IHook, PreDeleteDeletedEntityHook>();
            return this;
        }
    }
}