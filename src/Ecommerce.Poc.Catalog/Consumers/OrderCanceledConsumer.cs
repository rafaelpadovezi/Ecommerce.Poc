using DotNetCore.CAP;
using Ecommerce.Poc.Catalog.Dtos;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Ecommerce.Poc.Catalog.Consumers
{
    public class OrderCanceledConsumer : ICapSubscribe
    {
        private readonly ILogger<OrderCanceledConsumer> _logger;

        public OrderCanceledConsumer(ILogger<OrderCanceledConsumer> logger)
        {
            _logger = logger;
        }

        [CapSubscribe("order.canceled", Group = "catalog.order.canceled")]
        public Task UpdateOrderStockAsync(OrderCanceledMessage message)
        {
            _logger.LogInformation("Restore stock");
            return Task.CompletedTask;
        }
    }
}