using Catalog.API.Domain;
using Catalog.API.Repositories;

namespace Catalog.API.Services;

public class ProdutoService : IProdutoService
{
    private readonly IProdutoRepository _repository;

    public ProdutoService(IProdutoRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<Produto>> ListarAsync()
    {
        return await _repository.ListarAsync();
    }

    public async Task<Produto?> ObterPorIdAsync(string id)
    {
        return await _repository.ObterPorIdAsync(id);
    }

    public async Task CriarAsync(Produto produto)
    {
        await _repository.CriarAsync(produto);
    }
}