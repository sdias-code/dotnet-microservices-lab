using MassTransit;
using Pricing.API.Services;
using Shared.Contracts.Events;

namespace Pricing.API.Consumers;

public class ProductCreatedConsumer : IConsumer<ProductCreatedEvent>
{
    private readonly IPriceService _priceService;
    private readonly ILogger<ProductCreatedConsumer> _logger;

    public ProductCreatedConsumer(IPriceService priceService, ILogger<ProductCreatedConsumer> logger)
    {
        _priceService = priceService;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<ProductCreatedEvent> context)
    {
        var jsonEvent = context.Message;
        _logger.LogInformation("Received Product Created event. ID: {Id}, Name: {Name}", jsonEvent.Id, jsonEvent.Name);

        await _priceService.SavePriceAsync(jsonEvent.Id, 0.0m);

        _logger.LogInformation("Default initial price for product {Id} registered successfully in Redis.", jsonEvent.Id);
    }

}
