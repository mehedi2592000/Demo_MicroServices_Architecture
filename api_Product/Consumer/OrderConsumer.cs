using MassTransit;

namespace api_Product.Consumer
{
    public class OrderConsumer : IConsumer<OrderCreatedEvent>
    {
        private readonly ILogger<OrderCreatedEvent> _logger;

        public OrderConsumer(ILogger<OrderCreatedEvent> logger)
        {
            _logger = logger;
        }


        public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
        {
            var order = context.Message;
            Console.WriteLine($"Order received: {order.OrderId}, Product: {order.ProductId}, Total: {order.TotalPrice}");
            _logger.LogInformation("Fetching data: {@Data}",order);
            // Perform operations like updating product data.
            await Task.CompletedTask;
        }
    }
}
