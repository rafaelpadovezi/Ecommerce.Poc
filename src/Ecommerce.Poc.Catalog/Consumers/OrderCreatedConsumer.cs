using DotNetCore.CAP;
using Ecommerce.Poc.Catalog.Domain.Models;
using Ecommerce.Poc.Catalog.Dtos;
using Ecommerce.Poc.Catalog.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;
using DotNetCore.Cap.Idempotency;

namespace Ecommerce.Poc.Catalog.Consumers
{
    public class OrderCreatedConsumer : ICapSubscribe
    {
        private readonly IConsumerService<OrderCreatedMessage> _service;
        private readonly ILogger<OrderCreatedConsumer> _logger;

        public OrderCreatedConsumer(
            IConsumerService<OrderCreatedMessage> service,
            ILogger<OrderCreatedConsumer> logger)
        {
            _service = service;
            _logger = logger;
        }

        [CapSubscribe("order.created", Group = "catalog.order.created")]
        public async Task UpdateProductStock(OrderCreatedMessage message, [FromCap] CapHeader capHeader)
        {
            var fullMessage = ConsumerMessage.Create(capHeader, message);
            await _service.ProcessMessageAsync(fullMessage);
        }
    }
}