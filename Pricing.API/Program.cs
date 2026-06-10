using Pricing.API.Endpoints;
using Pricing.API.Infrastructure;
using Pricing.API.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
    options.InstanceName = "Pricing_";
});

builder.Services.AddScoped<IPrecoRepository, PrecoRepository>();
builder.Services.AddScoped<IPrecoService, PrecoService>();

var app = builder.Build();

app.MapPrecoEndpoints();

app.Run();