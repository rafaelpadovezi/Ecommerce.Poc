using System;
using System.Collections.Generic;

namespace Ecommerce.Poc.Sale
{
    public class Order
    {
        public Guid Id { get; set; }
        public DateTime CreationDate { get; set; } = DateTime.Now;
        public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
        public string OrderNumber { get; set; }
    }
}