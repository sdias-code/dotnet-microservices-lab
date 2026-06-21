using Catalog.API.Endpoints;
using Catalog.API.Infrastructure;
using Catalog.API.Infrastructure.Clients;
using Catalog.API.Repositories;
using Catalog.API.Services;
using MongoDB.Driver;
using Refit;
using Serilog;

// 1. CONFIGURAÇÃO INICIAL DO LOG COM SERILOG (Pode ser feita antes do builder para capturar logs de inicialização)
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
    .CreateLogger();

try
{
    Log.Information("Iniciando o host do Catalog.API...");

    var builder = WebApplication.CreateBuilder(args);

    // 2. ADICIONAR SERILOG AO HOST
    builder.Host.UseSerilog();

    // 3. REGISTRO DE SERVIÇOS (DI CONTAINER)

    // Banco de Dados (MongoDB)
    builder.Services.AddSingleton<IMongoClient>(sp =>
        new MongoClient(builder.Configuration.GetConnectionString("MongoDb")));

    builder.Services.AddSingleton<MongoContext>();

    // Health Checks (Utiliza o IMongoClient registrado acima)
    builder.Services.AddHealthChecks()
        .AddMongoDb(
            sp => sp.GetRequiredService<IMongoClient>(),
            name: "mongodb",
            tags: ["db", "data"]
        );

    // Camadas da Aplicação (Business / Data)
    builder.Services.AddScoped<IProdutoRepository, ProdutoRepository>();
    builder.Services.AddScoped<IProdutoService, ProdutoService>();

    // Integração HTTP Externa (Refit Client)
    var pricingUrl = builder.Configuration["PricingServiceUrl"] ?? "http://localhost:5002";
    builder.Services.AddRefitClient<IPrecoApiClient>()
        .ConfigureHttpClient(c => c.BaseAddress = new Uri(pricingUrl));

    // ----------------------------------------------------

    var app = builder.Build();

    // 4. PIPELINE DE MIDDLEWARES (A ordem aqui é estrita!)

    // O Log de Requisições DEVE vir antes dos Endpoints para capturar as métricas de tráfego
    app.UseSerilogRequestLogging();

    // Endpoints da Aplicação e Infraestrutura
    app.MapHealthChecks("/health");
    app.MapProdutoEndpoints();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "O host terminou inesperadamente!");
}
finally
{
    Log.CloseAndFlush();
}
