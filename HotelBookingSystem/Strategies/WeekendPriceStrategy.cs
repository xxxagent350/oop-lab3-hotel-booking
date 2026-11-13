namespace HotelsManager;

/// <summary>
/// Тариф выходного дня: ночи с пятницы по воскресенье стоят дороже.
/// Надбавка начисляется на каждую конкретную ночь.
/// </summary>
public sealed class WeekendPriceStrategy : IPriceStrategy
{
    public WeekendPriceStrategy(decimal markup = 0.20m)
    {
        if (markup < 0m)
            throw new ArgumentOutOfRangeException(nameof(markup), "Надбавка не может быть отрицательной.");

        Markup = markup;
    }

    /// <summary>Надбавка за ночь выходного дня (0.20 = +20%).</summary>
    public decimal Markup { get; }

    public string Name => $"Выходной тариф (+{Markup:P0})";

    public decimal CalculatePrice(decimal basePrice, int days, DateTime checkInDate)
    {
        PriceUtils.Validate(basePrice, days);

        decimal total = 0m;
        for (int i = 0; i < days; i++)
        {
            DateTime date = checkInDate.Date.AddDays(i);
            bool isWeekend = date.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday;
            total += isWeekend ? basePrice * (1m + Markup) : basePrice;
        }

        return total;
    }
}
