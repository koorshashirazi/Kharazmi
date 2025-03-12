using System.Threading.Tasks;

namespace Kharazmi.AspNetCore.Core.Tenancy
{
    public interface ITenantStore
    {
        Task<Tenant> FindTenantAsync(string tenantId);
    }
}