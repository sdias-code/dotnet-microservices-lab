using Pricing.API.Domain;

namespace Pricing.API.Infrastructure;

public interface IPriceRepository
{
    Task<Price?> GetByIdAsync(string productId);

    Task SaveAsync(Price price);
}
