using Catalogo.API.Contracts.Responses;
using Refit;

namespace Catalogo.API.Infrastructure.Clients;

public interface IPrecoApiClient
{
    [Get("/precos/{produtoId}")]
    Task<PrecoResponse> ObterPrecoPorIdAsync(string produtoId);
}