using System;
using Kharazmi.AspNetCore.Core.Dependency;

namespace Kharazmi.AspNetCore.Core.Tenancy
{
    public interface ITenantContainerFactory : ISingletonDependency
    {
        IServiceProvider CreateContainer(string tenantId);
    }
}