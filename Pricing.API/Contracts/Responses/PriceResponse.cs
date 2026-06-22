namespace Pricing.API.Contracts.Responses;

public record PriceResponse(
    string ProductId,
    decimal Value,
    string Currency
);
