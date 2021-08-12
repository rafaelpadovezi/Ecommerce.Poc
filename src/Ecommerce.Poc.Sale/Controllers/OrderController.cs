using DotNetCore.CAP;
using Ecommerce.Poc.Sale.Dtos;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace Ecommerce.Poc.Sale.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly SaleDbContext _context;
        private readonly ICapPublisher _capBus;

        public OrderController(
            ICapPublisher capPublisher,
            SaleDbContext context)
        {
            _context = context;
            _capBus = capPublisher;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] Order order)
        {
            await using (_context.Database.BeginTransaction(_capBus, autoCommit: true))
            {
                _context.Orders.Add(order);

                var orderMessage = new OrderMessage(
                    order.Items.Select(x => new OrderItemMessage(x.MaterialCode, x.Quantity)).ToList()
                );
                await _capBus.PublishAsync("order.created", orderMessage);

                await _context.SaveChangesAsync();
            }

            return Ok();
        }
    }
}