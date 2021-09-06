using System.Threading.Tasks;
using DotNetCore.CAP;
using Ecommerce.Poc.Catalog.Domain.Models;
using Ecommerce.Poc.Catalog.Dtos;
using Ecommerce.Poc.Catalog.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Poc.Catalog.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly ICapPublisher _capBus;
        private readonly CatalogDbContext _context;

        public ProductController(
            ICapPublisher capBus,
            CatalogDbContext context)
        {
            _capBus = capBus;
            _context = context;
        }
        
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Product newProduct)
        {
            await using (_context.Database.BeginTransaction(_capBus, autoCommit: true))
            {
                _context.Products.Add(newProduct);

                var message = new ProductCreatedMessage(
                    newProduct.Id,
                    newProduct.MaterialCode,
                    newProduct.Name,
                    newProduct.Description,
                    newProduct.Stock
                );
                await _capBus.PublishAsync("product.created", message);

                await _context.SaveChangesAsync();
            }

            return Ok();
        }
    }
}