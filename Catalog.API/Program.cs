using Catalog.API.Endpoints;
using Catalog.API.Infrastructure;
using Catalog.API.Infrastructure.Clients;
using Catalog.API.Repositories;
using Catalog.API.Services;
using MongoDB.Driver;
using Refit;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IMongoClient>(sp =>
    new MongoClient(
        builder.Configuration.GetConnectionString("MongoDb")
    ));

builder.Services.AddSingleton<MongoContext>();

builder.Services.AddScoped<IProdutoRepository, ProdutoRepository>();
builder.Services.AddScoped<IProdutoService, ProdutoService>();

builder.Services.AddRefitClient<IPrecoApiClient>()
    .ConfigureHttpClient(c =>
        c.BaseAddress = new Uri("http://localhost:5002"));

var app = builder.Build();

app.MapProdutoEndpoints();

app.Run();