using Pricing.API.Endpoints;
using Pricing.API.Infrastructure;
using Pricing.API.Services;
using Pricing.API.Consumers;
using MassTransit;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
    .CreateLogger();

try
{
    Log.Information("Starting Pricing.API host...");

    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog();

    // Distributed Cache (Redis)
    builder.Services.AddStackExchangeRedisCache(options =>
    {
        options.Configuration = builder.Configuration.GetConnectionString("Redis");
        options.InstanceName = "Pricing_";
    });

    // 🆕 Mastransit configuration
    builder.Services.AddMassTransit(x =>
    {
        // 1. Register the consumer
        x.AddConsumer<ProductCreatedConsumer>();

        x.UsingRabbitMq((context, cfg) =>
        {
            // 2. Configure the RabbitMQ address for the Docker environment.
            cfg.Host("rabbitmq", "/", h =>
            {
                h.Username("guest");
                h.Password("guest");
            });

            // 🆕 1. ACTIVATE THE NATIVE SCHEDULING OF RABBITMQ
            cfg.UseDelayedMessageScheduler();

            // 🆕 2. CONFIGURATION OF THE DLQ / REDELIVERY POLICY
            cfg.ReceiveEndpoint("Pricing.API.Consumers:ProductCreatedConsumer", e =>
            {
                // Level 1: If it fails, try 3 times in a row waiting 2 seconds between them
                e.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(2)));

                // Level 2 (Redelivery): If the 3 attempts fail, schedule the message retransmission.
                // It will attempt to reprocess 3 more times, but waiting 1 minute between each grand attempt.
                e.UseScheduledRedelivery(r => r.Intervals(
                    TimeSpan.FromMinutes(1),
                    TimeSpan.FromMinutes(2),
                    TimeSpan.FromMinutes(3)
                ));

                e.ConfigureConsumer<ProductCreatedConsumer>(context);
            });
        });
    });

    // Application Layers (Business / Data)
    builder.Services.AddScoped<IPriceRepository, PriceRepository>();
    builder.Services.AddScoped<IPriceService, PriceService>();

    // Health Checks (Redis)
    builder.Services.AddHealthChecks()
        .AddRedis(
            redisConnectionString: builder.Configuration.GetConnectionString("Redis")!,
            name: "redis",
            tags: ["cache"]
        );

    var app = builder.Build();

    // Middleware Pipeline (Order matters!)
    app.UseSerilogRequestLogging();

    // Application and Infrastructure Endpoints
    app.MapHealthChecks("/health");
    app.MapPriceEndpoints();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Pricing.API host terminated unexpectedly!");
}
finally
{
    Log.CloseAndFlush();
}
