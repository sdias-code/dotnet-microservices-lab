using Catalogo.API.Domain;
using MongoDB.Driver;

namespace Catalogo.API.Infrastructure;

public class MongoContext
{
    public IMongoCollection<Produto> Produtos { get; }

    public MongoContext(IMongoClient client)
    {
        var database = client.GetDatabase("CatalogoDb");

        Produtos = database.GetCollection<Produto>("Produtos");
    }
}