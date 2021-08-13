using System;
using System.Collections.Generic;

namespace Ecommerce.Poc.Sale.Dtos
{
    public record OrderItemMessage(string MaterialCode, int Quantity);

    public record OrderMessage(Guid Id, IEnumerable<OrderItemMessage> OrderItems);
}