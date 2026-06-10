using Precos.API.Contracts.Responses;

namespace Precos.API.Services;

public interface IPrecoService
{
    Task<PrecoResponse?> ObterPrecoAsync(string produtoId);

    Task SalvarPrecoAsync(string produtoId, decimal valor);
}