using Microsoft.EntityFrameworkCore;
using test.Models;

namespace test.Data
{
    public class TenantDbContext : DbContext
    {
        public TenantDbContext(DbContextOptions<TenantDbContext> options)
            : base(options)
        {
        }        

        public DbSet<Product> Products { get; set; }
    }
}