using Catalog.API.Contracts.Requests;
using Catalog.API.Contracts.Responses;
using Shared.Contracts.Events;
using Catalog.API.Infrastructure.Clients;
using Catalog.API.Services;
using Refit;
using MassTransit;
using MongoDB.Bson;

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
            var price = await priceClient.GetPriceByIdAsync(id);

            if (price is null)
            {
                return Results.Ok(new
                {
                    product.Id,
                    product.Name,
                    product.Description,
                    Warning = "Price not yet registered."
                });
            }

            var productResponse = new ProductResponse(
                product.Id,
                product.Name,
                product.Description,
                price.Value,
                price.Currency
            );

            return Results.Ok(productResponse);
        }
        catch (ApiException ex)
            when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return Results.Ok(new
            {
                product.Id,
                product.Name,
                product.Description,
                Warning = "Price not yet registered."
            });
        }
        catch (ApiException)
        {
            return Results.Problem(
                detail: "Pricing service unavailable.",
                statusCode: 503
            );
        }
    }
}
