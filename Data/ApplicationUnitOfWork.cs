using test.Interfaces;

namespace test.Data
{
    public class ApplicationUnitOfWork : IApplicationUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private readonly ITenantManagementRepository _tenantManagementRepository;

        public ApplicationUnitOfWork(ApplicationDbContext context, ITenantManagementRepository tenantManagementRepository)
        {
            _context = context;
            _tenantManagementRepository = tenantManagementRepository;
        }

        public ITenantManagementRepository TenantManagementRepository => _tenantManagementRepository;

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}