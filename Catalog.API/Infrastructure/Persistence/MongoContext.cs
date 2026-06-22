using Catalog.API.Domain;
using MongoDB.Driver;

namespace Catalog.API.Infrastructure;

public class MongoContext
{
    public IMongoCollection<Product> Products { get; }

    public MongoContext(IMongoClient client)
    {
        var database = client.GetDatabase("CatalogDb");

        Products = database.GetCollection<Product>("Products");
    }
}
