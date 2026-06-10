using Precos.API.Domain;

namespace Precos.API.Infrastructure;

public interface IPrecoRepository
{
    Task<Preco?> ObterPorIdAsync(string produtoId);

    Task SalvarAsync(Preco preco);
}
