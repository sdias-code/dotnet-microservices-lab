using System.Globalization;
using Microsoft.Extensions.Caching.Distributed;
using Precos.API.Domain;

namespace Precos.API.Infrastructure;

public class PrecoRepository : IPrecoRepository
{
    private readonly IDistributedCache _cache;

    public PrecoRepository(IDistributedCache cache)
    {
        _cache = cache;
    }

    public async Task<Preco?> ObterPorIdAsync(string produtoId)
    {
        var precoSalvo = await _cache.GetStringAsync(produtoId);

        if (string.IsNullOrEmpty(precoSalvo))
            return null;

        var sucesso = decimal.TryParse(
            precoSalvo,
            CultureInfo.InvariantCulture,
            out var valor
        );

        if (!sucesso)
            return null;

        return new Preco
        {
            ProdutoId = produtoId,
            Valor = valor,
            Moeda = "BRL"
        };
    }

    public async Task SalvarAsync(Preco preco)
    {
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1)
        };

        await _cache.SetStringAsync(
            preco.ProdutoId,
            preco.Valor.ToString(CultureInfo.InvariantCulture),
            options
        );
    }
}
