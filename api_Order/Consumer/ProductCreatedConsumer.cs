using MassTransit;
using MassTransit.Transports;

namespace api_Order.Consumer
{
    public class ProductCreatedConsumer : IConsumer<ProductCreatedEvent>
    {

        private readonly ILogger<ProductCreatedEvent> _logger;

        public ProductCreatedConsumer(ILogger<ProductCreatedEvent> logger)
        {
            _logger = logger;
        }


        public async Task Consume(ConsumeContext<ProductCreatedEvent> context)
        {
            try
            {
                var product = context.Message;
                Console.WriteLine($"[Consumer] Product received: {product.ProductId}, Name: {product.ProductName}, Price: {product.Price}");
                _logger.LogInformation("Product received: {@Data}", product);

                // Simulate processing logic
                await Task.Delay(500);
                _logger.LogInformation("Product processing completed successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ProductCreatedConsumer");
                throw;
            }
        }
    }
}
