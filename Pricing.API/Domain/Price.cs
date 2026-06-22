namespace Pricing.API.Domain;

public class Price
{
    public string ProductId { get; set; } = string.Empty;

    public decimal Value { get; set; }

    public string Currency { get; set; } = "BRL";
}
