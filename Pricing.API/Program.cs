using Pricing.API.Endpoints;
using Pricing.API.Infrastructure;
using Pricing.API.Services;
using Serilog;

// 1. CONFIGURAÇÃO INICIAL DO LOG COM SERILOG (Pode ser feita antes do builder para capturar logs de inicialização)
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
    .CreateLogger();

try
{
    Log.Information("Iniciando o host do Pricing.API...");

    var builder = WebApplication.CreateBuilder(args);

    // 2. ADICIONAR SERILOG AO HOST
    builder.Host.UseSerilog();

    // 3. REGISTRO DE SERVIÇOS (DI CONTAINER)

    // Cache Distribuído (Redis)
    builder.Services.AddStackExchangeRedisCache(options =>
    {
        options.Configuration = builder.Configuration.GetConnectionString("Redis");
        options.InstanceName = "Pricing_";
    });

    // Camadas da Aplicação (Business / Data)
    builder.Services.AddScoped<IPrecoRepository, PrecoRepository>();
    builder.Services.AddScoped<IPrecoService, PrecoService>();

    // Health Checks (Redis)
    builder.Services.AddHealthChecks()
        .AddRedis(
            redisConnectionString: builder.Configuration.GetConnectionString("Redis")!,
            name: "redis",
            tags: ["cache"]
        );

    // ----------------------------------------------------

    var app = builder.Build();

    // 4. PIPELINE DE MIDDLEWARES (A ordem aqui importa!)

    // O Log de Requisições DEVE vir antes dos Endpoints para capturar as métricas de tráfego
    app.UseSerilogRequestLogging();

    // Endpoints da Aplicação e Infraestrutura
    app.MapHealthChecks("/health");
    app.MapPrecoEndpoints();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "O host do Pricing.API terminou inesperadamente!");
}
finally
{
    Log.CloseAndFlush();
}
