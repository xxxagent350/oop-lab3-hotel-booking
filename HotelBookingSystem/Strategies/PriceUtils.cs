namespace HotelsManager;

/// <summary>Общие проверки входных данных для стратегий.</summary>
internal static class PriceUtils
{
    public static void Validate(decimal basePrice, int days)
    {
        if (basePrice < 0m)
            throw new ArgumentOutOfRangeException(nameof(basePrice), "Базовая цена не может быть отрицательной.");
        if (days <= 0)
            throw new ArgumentOutOfRangeException(nameof(days), "Количество ночей должно быть положительным.");
    }
}
