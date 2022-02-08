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
            _context.Orders.Add(order);

            var orderMessage = new OrderMessage(
                order.Id,
                order.Items.Select(x => new OrderItemMessage(x.MaterialCode, x.Quantity)).ToList()
            );

            await using (_context.Database.BeginTransaction(_capBus, autoCommit: true))
            {
                await _capBus.PublishAsync("order.created", orderMessage);

                await _context.SaveChangesAsync();
            }

            return Ok();
        }

        [HttpPost("cancel")]
        public async Task<IActionResult> CancelOrder([FromBody] Order order)
        {
            // cancel order

            var orderMessage = new OrderMessage(
                order.Id,
                order.Items.Select(x => new OrderItemMessage(x.MaterialCode, x.Quantity)).ToList()
            );

            await using (_context.Database.BeginTransaction(_capBus, autoCommit: true))
            {
                await _capBus.PublishAsync("order.canceled", orderMessage);

                await _context.SaveChangesAsync();
            }

            return Ok();
        }
    }
}