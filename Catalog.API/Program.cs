using Catalog.API.Endpoints;
using Catalog.API.Infrastructure;
using Catalog.API.Infrastructure.Clients;
using Catalog.API.Repositories;
using Catalog.API.Services;
using MongoDB.Driver;
using Refit;
using Serilog;
using MassTransit;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
    .CreateLogger();

try
{
    Log.Information("Starting Catalog.API host...");

    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog();

    // Database (MongoDB)
    builder.Services.AddSingleton<IMongoClient>(sp =>
        new MongoClient(builder.Configuration.GetConnectionString("MongoDb")));

    builder.Services.AddSingleton<MongoContext>();

    // Health Checks
    builder.Services.AddHealthChecks()
        .AddMongoDb(
            sp => sp.GetRequiredService<IMongoClient>(),
            name: "mongodb",
            tags: ["db", "data"]
        );

    // MassTransit 8.3.6 with RabbitMQ
    builder.Services.AddMassTransit(x =>
    {
        x.UsingRabbitMq((context, cfg) =>
        {
            var connectionString = builder.Configuration.GetConnectionString("RabbitMq")
                                   ?? "amqp://localhost:5672";

            cfg.Host(new Uri(connectionString));

            cfg.ConfigureEndpoints(context);
        });
    });

    // Application Layers
    builder.Services.AddScoped<IProductRepository, ProductRepository>();
    builder.Services.AddScoped<IProductService, ProductService>();

    // External HTTP Integration (Refit Client)
    var pricingUrl = builder.Configuration["PricingServiceUrl"] ?? "http://localhost:5002";
    builder.Services.AddRefitClient<IPriceApiClient>()
        .ConfigureHttpClient(c => c.BaseAddress = new Uri(pricingUrl));

    var app = builder.Build();

    // Middleware Pipeline
    app.UseSerilogRequestLogging();

    app.MapHealthChecks("/health");
    app.MapProductEndpoints();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly!");
}
finally
{
    Log.CloseAndFlush();
}
