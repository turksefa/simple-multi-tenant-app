using test.Interfaces;
using test.Models;

namespace test.Data
{
    public class TenantManagementRepository : Repository<Tenant>, ITenantManagementRepository
    {
        public TenantManagementRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}