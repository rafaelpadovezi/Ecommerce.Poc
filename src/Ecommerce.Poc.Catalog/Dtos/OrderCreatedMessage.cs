using System;
using System.Collections.Generic;
using DotNetCore.Cap.Idempotency;

namespace Ecommerce.Poc.Catalog.Dtos
{
    public record OrderItemMessage(string MaterialCode, int Quantity);

    public record OrderCreatedMessage(Guid Id, ICollection<OrderItemMessage> OrderItems) : IMessage
    {
        public string MessageId { get; set; }
        public string MessageGroup { get; set; }
    }

    public record OrderCanceledMessage(Guid Id, ICollection<OrderItemMessage> OrderItems);
}