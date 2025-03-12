
namespace Kharazmi.AspNetCore.Core.Tenancy
{
    public interface ITenantResolutionStrategy
    {
        string TenantId();
    }
   
}