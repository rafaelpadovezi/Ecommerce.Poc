using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Poc.Search.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductsController : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> Get(string term)
        {
            return Ok();
        }
    }
}