using System;

namespace Ecommerce.Poc.Sale
{
    public class OrderItem
    {
        public Guid Id { get; set; }
        public string MaterialCode { get; set; }
        public int Quantity { get; set; }
    }
}