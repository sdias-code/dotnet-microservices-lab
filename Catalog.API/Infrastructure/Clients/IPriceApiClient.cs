using Catalog.API.Contracts.Responses;
using Refit;

namespace Catalog.API.Infrastructure.Clients;

public interface IPrecoApiClient
{
    [Get("/precos/{produtoId}")]
    Task<PrecoResponse> ObterPrecoPorIdAsync(string produtoId);
}