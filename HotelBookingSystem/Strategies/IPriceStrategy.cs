namespace HotelsManager;

/// <summary>
/// Стратегия расчёта стоимости проживания (паттерн Strategy).
/// </summary>
public interface IPriceStrategy
{
    /// <summary>Человекочитаемое название тарифа.</summary>
    string Name { get; }

    /// <summary>
    /// Рассчитать полную стоимость проживания.
    /// </summary>
    /// <param name="basePrice">Базовая цена номера за одну ночь.</param>
    /// <param name="days">Количество ночей.</param>
    /// <param name="checkInDate">Дата заезда (для сезонных и выходных тарифов).</param>
    decimal CalculatePrice(decimal basePrice, int days, DateTime checkInDate);
}
