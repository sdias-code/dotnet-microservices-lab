namespace Catalog.API.Contracts.Responses;

public record ProductResponse(
    string? Id,
    string Name,
    string Description,
    decimal Price,
    string Currency
);
