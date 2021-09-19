using System;
using System.Collections.Generic;

namespace Ecommerce.Poc.Catalog.Dtos
{
    public record OrderItemMessage(string MaterialCode, int Quantity);

    public record OrderCreatedMessage(Guid Id, ICollection<OrderItemMessage> OrderItems)
    {
        public string MessageId { get; set; }
    }

    public record OrderCanceledMessage(Guid Id, ICollection<OrderItemMessage> OrderItems);
}