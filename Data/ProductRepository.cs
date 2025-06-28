using test.Interfaces;
using test.Models;

namespace test.Data
{
    public class ProductRepository : TenantRepository<Product>, IProductRepository
    {
        public ProductRepository(TenantDbContext context) : base(context)
        {
        }
    }
}