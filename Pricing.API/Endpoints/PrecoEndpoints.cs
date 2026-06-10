using Precos.API.Contracts.Requests;
using Precos.API.Services;

namespace Precos.API.Endpoints;

public static class PrecoEndpoints
{
    public static void MapPrecoEndpoints(this WebApplication app)
    {
        app.MapGet("/precos/{produtoId}", ObterPreco);

        app.MapPost("/precos", SalvarPreco);
    }

    private static async Task<IResult> ObterPreco(
        string produtoId,
        IPrecoService precoService)
    {
        var preco = await precoService.ObterPrecoAsync(produtoId);

        if (preco is null)
        {
            return Results.NotFound(new
            {
                Mensagem = "Preço não localizado para este produto no Redis."
            });
        }

        return Results.Ok(preco);
    }

    private static async Task<IResult> SalvarPreco(
        PrecoRequest request,
        IPrecoService precoService)
    {
        await precoService.SalvarPrecoAsync(
            request.ProdutoId,
            request.Valor
        );

        return Results.Created(
            $"/precos/{request.ProdutoId}",
            request
        );
    }
}