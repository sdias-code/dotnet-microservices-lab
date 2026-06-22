using Pricing.API.Contracts.Responses;
using Pricing.API.Domain;
using Pricing.API.Infrastructure;

namespace Pricing.API.Services;

public class PriceService : IPriceService
{
    private readonly IPriceRepository _repository;

    public PriceService(IPriceRepository repository)
    {
        _repository = repository;
    }

    public async Task<PriceResponse?> GetPriceAsync(string productId)
    {
        var price = await _repository.GetByIdAsync(productId);

        if (price is null)
            return null;

        return new PriceResponse(
            price.ProductId,
            price.Value,
            price.Currency
        );
    }

    public async Task SavePriceAsync(string productId, decimal value)
    {
        var price = new Price
        {
            ProductId = productId,
            Value = value,
            Currency = "BRL"
        };

        await _repository.SaveAsync(price);
    }
}
