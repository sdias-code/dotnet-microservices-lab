using Catalog.API.Domain;
using Catalog.API.Infrastructure;
using MongoDB.Driver;

namespace Catalog.API.Repositories;

public class ProdutoRepository : IProdutoRepository
{
    private readonly MongoContext _context;

    public ProdutoRepository(MongoContext context)
    {
        _context = context;
    }

    public async Task<List<Produto>> ListarAsync()
    {
        return await _context.Produtos
            .Find(_ => true)
            .ToListAsync();
    }

    public async Task<Produto?> ObterPorIdAsync(string id)
    {
        return await _context.Produtos
            .Find(p => p.Id == id)
            .FirstOrDefaultAsync();
    }

    public async Task CriarAsync(Produto produto)
    {
        await _context.Produtos.InsertOneAsync(produto);
    }
}
