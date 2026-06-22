namespace Catalog.API.Contracts.Responses;

public record PriceResponse(string ProductId, decimal Value, string Currency);
