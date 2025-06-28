using test.Models;

namespace test.Interfaces
{
    public interface ITenantService
    {
        Task<Tenant> GetCurrentTenantAsync();
        Task<string> GetCurrentTenantConnectionStringAsync();
        Task<bool> HasValidTenantAsync();
        Task<string> GetCurrentUserIdAsync();
        void ClearCache(string userId);
    }
}