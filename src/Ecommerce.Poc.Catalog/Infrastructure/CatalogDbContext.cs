using DotNetCore.CAP.Messages;
using Ecommerce.Poc.Catalog.Domain.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

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

        public override int SaveChanges()
        {
            try
            {
                return base.SaveChanges();
            }
            catch (DbUpdateException ex) when (IsMessageExistsError(ex))
            {
                // If is unique constraint error it means that the message
                // was already processed and should do nothing
                return 0;
            }
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            try
            {
                return base.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException ex) when (IsMessageExistsError(ex))
            {
                // If is unique constraint error it means that the message
                // was already processed and should do nothing
                return Task.FromResult(0);
            }
        }

        private static bool IsMessageExistsError(DbUpdateException ex)
        {
            if (ex.InnerException is not SqlException sqlEx)
                return false;

            var entry = ex.Entries.FirstOrDefault(
                x => x.Entity.GetType() == typeof(MessageTracking));
            // SqlServer: Error 2627
            // Violation of PRIMARY KEY constraint Constraint Name.
            // Cannot insert duplicate key in object Table Name.
            return sqlEx.Number == 2627 && entry is not null;
        }
    }
}