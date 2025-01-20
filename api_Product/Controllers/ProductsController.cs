using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using api_Product.CommonFilter;
using MassTransit;
using System.Diagnostics;

namespace api_Product.Controllers
{
    [Route("api/Products")]
    [ApiController]
    public class ProductsController : ControllerBase
    {

        private readonly IBus _bus;
        private readonly ILogger<ProductsController> _logger;
        private static readonly ActivitySource ActivitySource = new("ProductApi");

        public ProductsController(IBus bus, ILogger<ProductsController> logger)
        {
            _bus = bus;
            _logger=logger;
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

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct(int id)
        {
            using var activity = ActivitySource.StartActivity("GetProduct");
            _logger.LogInformation("Getting product by id");

            var product = new Product();
            product=null;
            if (product == null)
            {
                activity?.SetTag("Status", "NotFound");
                _logger.LogInformation("Status", "NotFound");
                return NotFound();
            }

            _logger.LogInformation("Status", "Success");
            return Ok(product);
        }

        //[HttpGet("{id}")]
        //public Student Get(int id)
        //{
        //    _logger.LogInformation("Getting student by id");
        //    return _context.Students.Find(id);
        //}
    }
}
