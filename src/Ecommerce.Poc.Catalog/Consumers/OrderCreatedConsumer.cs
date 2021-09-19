using DotNetCore.CAP;
using Ecommerce.Poc.Catalog.Dtos;
using System.Threading.Tasks;
using DotNetCore.Cap.Idempotency;

namespace Ecommerce.Poc.Catalog.Consumers
{
    public class OrderCreatedConsumer : ICapSubscribe
    {
        private readonly IConsumerService<OrderCreatedMessage> _service;

        public OrderCreatedConsumer(IConsumerService<OrderCreatedMessage> service) =>
            _service = service;

        [CapSubscribe("order.created", Group = "catalog.order.created")]
        public async Task UpdateProductStock(OrderCreatedMessage message)
        {
            await _service.ProcessMessageAsync(message);
        }
    }
}