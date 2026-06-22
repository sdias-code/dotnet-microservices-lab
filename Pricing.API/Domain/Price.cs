namespace Pricing.API.Domain;

public class Preco
{
    public string ProdutoId { get; set; } = string.Empty;

    public decimal Valor { get; set; }

    public string Moeda { get; set; } = "BRL";
}
