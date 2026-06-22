using Catalog.API.Domain;

namespace Catalog.API.Services;

public interface IProductService
{
    Task<List<Product>> GetAllAsync();

    Task<Product?> GetByIdAsync(string id);

    Task CreateAsync(Product product);
}
