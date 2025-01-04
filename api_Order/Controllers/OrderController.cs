using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace api_Order.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IBus _bus;

        public OrderController(IBus bus)
        {
            _bus = bus;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] Order order)
        {
            // Simulate saving order to the database.
            order.OrderId = Guid.NewGuid();
            order.TotalPrice = order.Quantity * 100; // Assume fixed price for simplicity.

            // Publish OrderCreatedEvent.
            await _bus.Publish(new OrderCreatedEvent
            {
                OrderId = order.OrderId,
                ProductId = order.ProductId,
                Quantity = order.Quantity,
                TotalPrice = order.TotalPrice
            });

            return Ok(order);
        }
    }
}
