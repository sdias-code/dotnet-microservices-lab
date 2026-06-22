using Catalog.API.Contracts.Requests;
using Catalog.API.Contracts.Responses;
using Catalog.API.Infrastructure.Clients;
using Catalog.API.Services;
using Refit;

namespace Catalog.API.Endpoints;

public static class ProdutoEndpoints
{
    public static void MapProdutoEndpoints(this WebApplication app)
    {
        app.MapGet("/produtos", ListarProdutos);

        app.MapPost("/produtos", CriarProduto);

        app.MapGet("/detalhes-produto/{id}", ObterDetalhesProduto);
    }

    private static async Task<IResult> ListarProdutos(
        IProdutoService produtoService)
    {
        var produtos = await produtoService.ListarAsync();

        return Results.Ok(produtos);
    }

    private static async Task<IResult> CriarProduto(
        ProdutoRequest request,
        IProdutoService produtoService)
    {
        var produto = new Domain.Produto
        {
            Nome = request.Nome,
            Descricao = request.Descricao
        };

        await produtoService.CriarAsync(produto);

        return Results.Created(
            $"/detalhes-produto/{produto.Id}",
            produto
        );
    }

    private static async Task<IResult> ObterDetalhesProduto(
        string id,
        IProdutoService produtoService,
        IPrecoApiClient precoClient)
    {
        var produto = await produtoService.ObterPorIdAsync(id);

        if (produto is null)
        {
            return Results.NotFound(new
            {
                Mensagem = "Produto não encontrado no MongoDB."
            });
        }

        try
        {
            var preco = await precoClient.ObterPrecoPorIdAsync(id);

            if (preco is null)
            {
                return Results.Ok(new
                {
                    produto.Id,
                    produto.Nome,
                    produto.Descricao,
                    Aviso = "Preço ainda não cadastrado."
                });
            }

            var produtoResponse = new ProdutoResponse(
                produto.Id,
                produto.Nome,
                produto.Descricao,
                preco.Valor,
                preco.Moeda
            );

            return Results.Ok(produtoResponse);
        }
        catch (ApiException ex)
            when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return Results.Ok(new
            {
                produto.Id,
                produto.Nome,
                produto.Descricao,
                Aviso = "Preço ainda não cadastrado."
            });
        }
        catch (ApiException)
        {
            return Results.Problem(
                detail: "Serviço de preços indisponível.",
                statusCode: 503
            );
        }
    }
}


