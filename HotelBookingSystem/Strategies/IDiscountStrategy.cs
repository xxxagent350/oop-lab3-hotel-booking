namespace HotelsManager;

/// <summary>
/// Стратегия скидки (паттерн Strategy): отделяет правило скидки от расчёта брони.
/// </summary>
public interface IDiscountStrategy
{
    string Name { get; }

    /// <summary>Рассчитать размер скидки в рублях.</summary>
    decimal CalculateDiscount(decimal subtotal, int nights);
}
