using System.Linq;
using System.Threading.Tasks;
using Ecommerce.Poc.Catalog.Dtos;
using Ecommerce.Poc.Catalog.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Ziggurat;

namespace Ecommerce.Poc.Catalog.Domain.Services
{
    public class OrderCreatedService : IConsumerService<OrderCreatedMessage>
    {
        private readonly CatalogDbContext _context;
        private readonly ILogger<OrderCreatedService> _logger;

        public OrderCreatedService(CatalogDbContext context, ILogger<OrderCreatedService> logger)
        {
            _context = context;
            _logger = logger;
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

            await Task.Delay(250);

            try
            {

            }
            catch(DbUpdateException)
            {
                _logger.LogWarning("[TESTE]Mensagem {messageId} duplicada", message.Id);
                throw;
            }
            await _context.SaveChangesAsync();
            _logger.LogWarning("[TESTE]Mensagem {messageId} processada com sucesso", message.Id);
        }
    }
}