using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using api_Product.CommonFilter;
using MassTransit;

namespace api_Product.Controllers
{
    [Route("api/Products")]
    [ApiController]
    public class ProductsController : ControllerBase
    {

        private readonly IBus _bus;


        public ProductsController(IBus bus)
        {
            _bus = bus;
        }

        [Authorize]       
        [HttpGet("getproduct")]
        [RouteBaseActionFilterData]
        public async Task<IActionResult> GetList()
        {
            List<int> list = new List<int>() { 1,2,3,4,5,6};

            return Ok(list);    
        }

        [Authorize]
        [HttpGet("getproductstring")]
        [RouteBaseActionFilterData]
        public async Task<IActionResult> Getproductstring()
        {
            List<string> list = new List<string>() { "car", "light", "bulb", "tyer" };

            return Ok(list);
        }

        [AllowAnonymous]
        [HttpGet("public")]
        public IActionResult PublicEndpoint()
        {
            return Ok("This endpoint is accessible to everyone.");
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] Product product)
        {
            // Simulate saving product to the database.
            product.ProductId = Guid.NewGuid();

            // Publish ProductCreatedEvent.
            await _bus.Publish(new ProductCreatedEvent
            {
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                Price = product.Price
            });

            Console.WriteLine("it sworks");

            return Ok(product);
        }
    }
}
