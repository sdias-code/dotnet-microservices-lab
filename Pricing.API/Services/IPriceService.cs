using Pricing.API.Contracts.Responses;

namespace Pricing.API.Services;

public interface IPrecoService
{
    Task<PrecoResponse?> ObterPrecoAsync(string produtoId);

    Task SalvarPrecoAsync(string produtoId, decimal valor);
}