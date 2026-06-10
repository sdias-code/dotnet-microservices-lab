using Catalogo.API.Domain;

namespace Catalogo.API.Repositories;

public interface IProdutoRepository
{
    Task<List<Produto>> ListarAsync();

    Task<Produto?> ObterPorIdAsync(string id);

    Task CriarAsync(Produto produto);
}
