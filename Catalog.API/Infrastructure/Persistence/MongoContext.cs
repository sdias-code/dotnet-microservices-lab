using Catalog.API.Domain;
using MongoDB.Driver;

namespace Catalog.API.Infrastructure;

public class MongoContext
{
    public IMongoCollection<Produto> Produtos { get; }

    public MongoContext(IMongoClient client)
    {
        var database = client.GetDatabase("CatalogoDb");

        Produtos = database.GetCollection<Produto>("Produtos");
    }
}