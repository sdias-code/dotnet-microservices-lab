using Precos.API.Contracts.Responses;
using Precos.API.Domain;
using Precos.API.Infrastructure;

namespace Precos.API.Services;

public class PrecoService : IPrecoService
{
    private readonly IPrecoRepository _repository;

    public PrecoService(IPrecoRepository repository)
    {
        _repository = repository;
    }

    public async Task<PrecoResponse?> ObterPrecoAsync(string produtoId)
    {
        var preco = await _repository.ObterPorIdAsync(produtoId);

        if (preco is null)
            return null;

        return new PrecoResponse(
            preco.ProdutoId,
            preco.Valor,
            preco.Moeda
        );
    }

    public async Task SalvarPrecoAsync(string produtoId, decimal valor)
    {
        var preco = new Preco
        {
            ProdutoId = produtoId,
            Valor = valor,
            Moeda = "BRL"
        };

        await _repository.SalvarAsync(preco);
    }
}