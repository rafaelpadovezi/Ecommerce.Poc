using DotNetCore.CAP;
using Ecommerce.Poc.Catalog.Dtos;
using Ecommerce.Poc.Catalog.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;

namespace Ecommerce.Poc.Catalog.Consumers
{
    public class OrderCreatedConsumer : ICapSubscribe
    {
        private readonly CatalogDbContext _context;
        private readonly ILogger<OrderCreatedConsumer> _logger;

        public OrderCreatedConsumer(
            CatalogDbContext context,
            ILogger<OrderCreatedConsumer> logger)
        {
            _context = context;
            _logger = logger;
        }

        [CapSubscribe("order.created")]
        public async Task UpdateProductStock(OrderCreatedMessage message)
        {
            _logger.LogDebug("Message received from customer {message}", message);
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