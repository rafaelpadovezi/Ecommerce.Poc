using System;
using System.Collections.Generic;

namespace Ecommerce.Poc.Search.Dtos
{
    public record OrderItemMessage(string MaterialCode, int Quantity);

    public record OrderCreatedMessage(Guid Id, ICollection<OrderItemMessage> OrderItems);
}