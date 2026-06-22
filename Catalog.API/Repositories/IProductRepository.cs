using Catalog.API.Domain;

namespace Catalog.API.Repositories;

public interface IProdutoRepository
{
    Task<List<Produto>> ListarAsync();

    Task<Produto?> ObterPorIdAsync(string id);

    Task CriarAsync(Produto produto);
}
