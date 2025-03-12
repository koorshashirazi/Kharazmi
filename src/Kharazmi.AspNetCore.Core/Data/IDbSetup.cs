using Kharazmi.AspNetCore.Core.Dependency;

namespace Kharazmi.AspNetCore.Core.Data
{
    public interface IDbSetup : IScopedDependency
    {
        void Seed();
    }
}