using Pricing.API.Domain;

namespace Pricing.API.Infrastructure;

public interface IPrecoRepository
{
    Task<Preco?> ObterPorIdAsync(string produtoId);

    Task SalvarAsync(Preco preco);
}
