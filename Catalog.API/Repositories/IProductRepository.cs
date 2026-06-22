using Catalog.API.Domain;

namespace Catalog.API.Repositories;

public interface IProductRepository
{
    Task<List<Product>> GetAllAsync();

    Task<Product?> GetByIdAsync(string id);

    Task CreateAsync(Product product);
}
