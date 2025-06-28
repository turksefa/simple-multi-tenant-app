using test.Interfaces;

namespace test.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly TenantDbContext _context;

        public UnitOfWork(TenantDbContext context)
        {
            _context = context;
            Products = new ProductRepository(_context);
        }

        public IProductRepository Products { get; private set; }

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