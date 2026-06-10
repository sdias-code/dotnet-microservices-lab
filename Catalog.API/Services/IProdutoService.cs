using Catalogo.API.Domain;

namespace Catalogo.API.Services;

public interface IProdutoService
{
    Task<List<Produto>> ListarAsync();

    Task<Produto?> ObterPorIdAsync(string id);

    Task CriarAsync(Produto produto);
}