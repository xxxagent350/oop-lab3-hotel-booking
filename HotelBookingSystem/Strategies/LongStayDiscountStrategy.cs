namespace HotelsManager;

/// <summary>Скидка за длительное проживание: 7+ ночей — 10%, 14+ ночей — 15%.</summary>
public sealed class LongStayDiscountStrategy : IDiscountStrategy
{
    public string Name => "Длительное проживание (10%/15%)";

    public decimal CalculateDiscount(decimal subtotal, int nights)
    {
        if (subtotal <= 0m || nights <= 0)
            return 0m;

        if (nights >= 14)
            return subtotal * 0.15m;
        if (nights >= 7)
            return subtotal * 0.10m;

        return 0m;
    }
}
