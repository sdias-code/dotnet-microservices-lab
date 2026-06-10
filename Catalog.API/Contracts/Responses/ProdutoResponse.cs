namespace Catalogo.API.Contracts.Responses;

public record ProdutoResponse(
    string? Id,
    string Nome,
    string Descricao,
    decimal Preco,
    string Moeda
);