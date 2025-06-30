using test.Interfaces;

namespace test.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly TenantDbContext _context;
        private readonly IProductRepository _productRepository;

        public UnitOfWork(TenantDbContext context, IProductRepository productRepository)
        {
            _context = context;
            _productRepository = productRepository;
        }

        public IProductRepository Products => _productRepository;

        public void Dispose()
        {
            _context?.Dispose();
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}