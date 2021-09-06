using System;

namespace Ecommerce.Poc.Search.Models
{
    public class Product
    {
        public Guid Id { get; set; }
        public string MaterialCode { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int TotalSales { get; set; } = 0;
    }
}