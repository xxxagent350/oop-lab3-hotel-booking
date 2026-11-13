namespace HotelsManager;

/// <summary>
/// Сезонный тариф: коэффициент зависит от месяца каждой ночи.
/// Лето и новогодние праздники — высокий сезон.
/// </summary>
public sealed class SeasonalPriceStrategy : IPriceStrategy
{
    private static readonly IReadOnlyDictionary<int, decimal> Coefficients = new Dictionary<int, decimal>
    {
        [1] = 1.15m,   // новогодние праздники
        [6] = 1.30m,   // лето
        [7] = 1.35m,
        [8] = 1.30m,
        [12] = 1.25m   // новогодние праздники
    };

    public string Name => "Сезонный тариф";

    public decimal CalculatePrice(decimal basePrice, int days, DateTime checkInDate)
    {
        PriceUtils.Validate(basePrice, days);

        decimal total = 0m;
        for (int i = 0; i < days; i++)
            total += basePrice * CoefficientFor(checkInDate.Date.AddDays(i));

        return total;
    }

    /// <summary>Сезонный коэффициент для конкретной даты.</summary>
    public static decimal CoefficientFor(DateTime date)
        => Coefficients.TryGetValue(date.Month, out decimal coefficient) ? coefficient : 1m;
}
