using Catalog.API.Contracts.Responses;
using Refit;

namespace Catalog.API.Infrastructure.Clients;

public interface IPriceApiClient
{
    [Get("/prices/{productId}")]
    Task<PriceResponse> GetPriceByIdAsync(string productId);
}
