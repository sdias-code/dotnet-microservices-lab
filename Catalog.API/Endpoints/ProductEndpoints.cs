using Catalog.API.Contracts.Requests;
using Catalog.API.Contracts.Responses;
using Shared.Contracts.Events;
using Catalog.API.Infrastructure.Clients;
using Catalog.API.Services;
using Refit;
using MassTransit;
using MongoDB.Bson;
using Polly.Timeout;

namespace Catalog.API.Endpoints;

public static class ProductEndpoints
{
    public static void MapProductEndpoints(this WebApplication app)
    {
        app.MapGet("/products", ListProducts);

        app.MapPost("/products", CreateProduct);

        app.MapGet("/products/{id}", GetProductDetails);
    }

    private static async Task<IResult> ListProducts(
        IProductService productService)
    {
        var products = await productService.GetAllAsync();

        return Results.Ok(products);
    }

    private static async Task<IResult> CreateProduct(
        ProductRequest request,
        IProductService productService,
        IPublishEndpoint publishEndpoint)
    {
        var product = new Domain.Product
        {
            Name = request.Name,
            Description = request.Description
        };

        await productService.CreateAsync(product);

        await publishEndpoint.Publish(new ProductCreatedEvent(
            product.Id!,
            product.Name,
            product.Description
        ));

        return Results.Created(
            $"/products/{product.Id}",
            product
        );
    }

    private static async Task<IResult> GetProductDetails(
        string id,
        IProductService productService,
        IPriceApiClient priceClient)
    {
        if (!ObjectId.TryParse(id, out _))
        {
            return Results.BadRequest(new
            {
                Error = "Invalid ID Format",
                Message = "The product ID must be a valid 24-digit hexadecimal string."
            });
        }

        var product = await productService.GetByIdAsync(id);

        if (product is null)
        {
            return Results.NotFound(new
            {
                Message = "Product not found in MongoDB."
            });
        }

        try
        {
            // Resilient call via Polly + Refit
            var price = await priceClient.GetPriceByIdAsync(id);

            if (price is null)
            {
                return Results.Ok(new { product.Id, product.Name, product.Description, Notice = "Price not registered yet." });
            }

            var productResponse = new ProductResponse(product.Id, product.Name, product.Description, price.Value, price.Currency);
            return Results.Ok(productResponse);
        }
        // Scenario 1: The service responded, but with an error (e.g., 404 Not Found)
        catch (ApiException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return Results.Ok(new { product.Id, product.Name, product.Description, Notice = "Price not registered yet." });
        }
        // Scenario 2: Any other HTTP error returned by the pricing service (e.g., 500)
        catch (ApiException)
        {
            return Results.Problem(detail: "Pricing service returned an error.", statusCode: 502);
        }

        // 🆕 Scenario 3: The container is down or the network failed (Captures the HttpRequestException you received!)
        // 🆕 Scenario 4: The timeout expired (Polly Timeout via TaskCanceledException)
        // 🆕 Scenario 5: The timeout expired (Polly Timeout via TimeoutRejectedException)
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or TimeoutRejectedException)
        {
            return Results.Problem(
                detail: "Pricing service is completely unavailable or a global network timeout occurred.",
                statusCode: 503
            );
        }



    }
}
