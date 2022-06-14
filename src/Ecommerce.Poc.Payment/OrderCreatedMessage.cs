using Ziggurat;

namespace Ecommerce.Poc.Payment;

public record OrderCreatedMessage(Guid Id, ICollection<OrderItemMessage> OrderItems) : IMessage
{
    public string? MessageId { get; set; }
    public string MessageGroup { get; set; }
}

public record OrderItemMessage(string MaterialCode, int Quantity);