using CliFx;
using CliFx.Attributes;
using Ecommerce.Poc.Catalog.Domain.Models;
using Ecommerce.Poc.Catalog.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace Ecommerce.Poc.Catalog.Commands
{
    [Command("seed")]
    public class SeedCommand : ICommand
    {
        public async ValueTask ExecuteAsync(IConsole console)
        {
            var connectionString = Program.Configuration.GetConnectionString("CatalogDbContext");
            var dbContext = CatalogDbContext.CreateContext(connectionString);

            var pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync();
            if (pendingMigrations.ToList().Any())
            {
                await dbContext.Database.MigrateAsync();
                await console.Output.WriteLineAsync("Migrate executed!");
            }

            dbContext.Products.AddRange(
                new Product("1111", 100),
                new Product("2222", 50),
                new Product("3333", 75));
            await dbContext.SaveChangesAsync();
            await console.Output.WriteLineAsync("Seed has been executed!");
        }
    }
}