using System.Linq;
using System.Threading.Tasks;
using Ecommerce.Poc.Catalog.Dtos;
using Ecommerce.Poc.Catalog.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Ziggurat;

namespace Ecommerce.Poc.Catalog.Domain.Services
{
    public class OrderCreatedService : IConsumerService<OrderCreatedMessage>
    {
        private readonly CatalogDbContext _context;

        public OrderCreatedService(CatalogDbContext context)
        {
            _context = context;
        }

        public async Task ProcessMessageAsync(OrderCreatedMessage message)
        {
            var materialCodes = message.OrderItems.Select(x => x.MaterialCode);
            var productQuantities = message.OrderItems.ToDictionary(x => x.MaterialCode, x => x.Quantity);

            var productsToUpdate = await _context.Products
                .Where(x => materialCodes.Contains(x.MaterialCode))
                .ToListAsync();

            foreach (var product in productsToUpdate)
            {
                product.Stock -= productQuantities[product.MaterialCode];
            }

            await _context.SaveChangesAsync();
        }
    }
}