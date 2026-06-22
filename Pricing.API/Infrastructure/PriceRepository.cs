using System.Globalization;
using Microsoft.Extensions.Caching.Distributed;
using Pricing.API.Domain;

namespace Pricing.API.Infrastructure;

public class PriceRepository : IPriceRepository
{
    private readonly IDistributedCache _cache;

    public PriceRepository(IDistributedCache cache)
    {
        _cache = cache;
    }

    public async Task<Price?> GetByIdAsync(string productId)
    {
        var savedPrice = await _cache.GetStringAsync(productId);

        if (string.IsNullOrEmpty(savedPrice))
            return null;

        var success = decimal.TryParse(
            savedPrice,
            CultureInfo.InvariantCulture,
            out var value
        );

        if (!success)
            return null;

        return new Price
        {
            ProductId = productId,
            Value = value,
            Currency = "BRL"
        };
    }

    public async Task SaveAsync(Price price)
    {
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1)
        };

        await _cache.SetStringAsync(
            price.ProductId,
            price.Value.ToString(CultureInfo.InvariantCulture),
            options
        );
    }
}
