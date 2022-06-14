using DotNetCore.CAP;
using Ziggurat;

namespace Ecommerce.Poc.Payment;

public class OrderCreatedConsumer : ICapSubscribe
{
    private readonly IConsumerService<OrderCreatedMessage> _service;

    public OrderCreatedConsumer(IConsumerService<OrderCreatedMessage> service)
    {
        _service = service;
    }
    
    [CapSubscribe("order.created", Group = "payment.order.created")]
    public async Task ConsumeMessage(OrderCreatedMessage message)
    {
        await _service.ProcessMessageAsync(message);
    }
}