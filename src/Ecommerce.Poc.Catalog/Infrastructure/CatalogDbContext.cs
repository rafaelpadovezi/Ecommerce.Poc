using Ecommerce.Poc.Catalog.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Ziggurat.SqlServer;

namespace Ecommerce.Poc.Catalog.Infrastructure
{
    public class CatalogDbContext : DbContext
    {
        public CatalogDbContext(DbContextOptions<CatalogDbContext> options)
            : base(options)
        {
        }

        public static CatalogDbContext CreateContext(string connectionString)
        {
            var optionsBuilder = new DbContextOptionsBuilder<CatalogDbContext>();
            optionsBuilder.UseSqlServer(connectionString);

            return new CatalogDbContext(optionsBuilder.Options);
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<MessageTracking> Messages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.MapMessageTracker();
            base.OnModelCreating(modelBuilder);
        }
    }
}