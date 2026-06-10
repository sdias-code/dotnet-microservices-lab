using Precos.API.Endpoints;
using Precos.API.Infrastructure;
using Precos.API.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
    options.InstanceName = "Precos_";
});

builder.Services.AddScoped<IPrecoRepository, PrecoRepository>();
builder.Services.AddScoped<IPrecoService, PrecoService>();

var app = builder.Build();

app.MapPrecoEndpoints();

app.Run();