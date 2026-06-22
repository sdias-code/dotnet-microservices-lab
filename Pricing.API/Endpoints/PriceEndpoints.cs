using Pricing.API.Contracts.Requests;
using Pricing.API.Services;

namespace Pricing.API.Endpoints;

public static class PriceEndpoints
{
    public static void MapPriceEndpoints(this WebApplication app)
    {
        app.MapGet("/prices/{productId}", GetPrice);

        app.MapPost("/prices", SavePrice);
    }

    private static async Task<IResult> GetPrice(
        string productId,
        IPriceService priceService)
    {
        var price = await priceService.GetPriceAsync(productId);

        if (price is null)
        {
            return Results.NotFound(new
            {
                Message = "Price not found for this product in Redis."
            });
        }

        return Results.Ok(price);
    }

    private static async Task<IResult> SavePrice(
        PriceRequest request,
        IPriceService priceService)
    {
        await priceService.SavePriceAsync(
            request.ProductId,
            request.Value
        );

        return Results.Created(
            $"/prices/{request.ProductId}",
            request
        );
    }
}
