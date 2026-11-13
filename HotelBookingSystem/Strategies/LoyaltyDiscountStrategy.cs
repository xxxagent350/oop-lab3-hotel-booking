namespace HotelsManager;

/// <summary>Скидка постоянного гостя — 5%.</summary>
public sealed class LoyaltyDiscountStrategy : IDiscountStrategy
{
    public LoyaltyDiscountStrategy(decimal percent = 0.05m)
    {
        if (percent < 0m || percent > 1m)
            throw new ArgumentOutOfRangeException(nameof(percent), "Скидка должна быть в диапазоне 0..1.");

        Percent = percent;
    }

    public decimal Percent { get; }

    public string Name => $"Постоянный гость (-{Percent:P0})";

    public decimal CalculateDiscount(decimal subtotal, int nights)
        => subtotal <= 0m ? 0m : subtotal * Percent;
}
