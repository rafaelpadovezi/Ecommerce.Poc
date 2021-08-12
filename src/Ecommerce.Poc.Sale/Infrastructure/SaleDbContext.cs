using Microsoft.EntityFrameworkCore;
using Ecommerce.Poc.Sale;

namespace Ecommerce.Poc.Sale
{
    public class SaleDbContext : DbContext
    {
        public SaleDbContext(DbContextOptions<SaleDbContext> options)
            : base(options)
        {
        }

        public DbSet<Order> Orders { get; set; }
    }
}