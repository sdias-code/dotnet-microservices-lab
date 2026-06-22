using Catalog.API.Domain;
using Catalog.API.Infrastructure;
using MongoDB.Driver;

namespace Catalog.API.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly MongoContext _context;

    public ProductRepository(MongoContext context)
    {
        _context = context;
    }

    public async Task<List<Product>> GetAllAsync()
    {
        return await _context.Products
            .Find(_ => true)
            .ToListAsync();
    }

    public async Task<Product?> GetByIdAsync(string id)
    {
        return await _context.Products
            .Find(p => p.Id == id)
            .FirstOrDefaultAsync();
    }

    public async Task CreateAsync(Product product)
    {
        await _context.Products.InsertOneAsync(product);
    }
}
