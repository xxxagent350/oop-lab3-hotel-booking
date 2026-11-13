namespace HotelsManager;

/// <summary>Обычный тариф: базовая цена умножается на количество ночей.</summary>
public sealed class RegularPriceStrategy : IPriceStrategy
{
    public string Name => "Обычный тариф";

    public decimal CalculatePrice(decimal basePrice, int days, DateTime checkInDate)
    {
        PriceUtils.Validate(basePrice, days);
        return basePrice * days;
    }
}
