using System;

namespace Ecommerce.Poc.Catalog.Domain.Models
{
    public class Product
    {
        public Product(string materialCode, int stock)
        {
            MaterialCode = materialCode;
            Stock = stock;
        }

        public Guid Id { get; set; }
        public string MaterialCode { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Stock { get; set; }
    }
}