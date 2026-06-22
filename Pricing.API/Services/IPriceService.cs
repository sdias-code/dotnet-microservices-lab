using Pricing.API.Contracts.Responses;

namespace Pricing.API.Services;

public interface IPriceService
{
    Task<PriceResponse?> GetPriceAsync(string productId);

    Task SavePriceAsync(string productId, decimal value);
}
