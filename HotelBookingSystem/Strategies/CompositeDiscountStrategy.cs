namespace HotelsManager;

/// <summary>
/// Композитная скидка: позволяет применять несколько правил скидок одновременно.
/// </summary>
public sealed class CompositeDiscountStrategy : IDiscountStrategy
{
    private readonly IReadOnlyList<IDiscountStrategy> _strategies;

    public CompositeDiscountStrategy(params IDiscountStrategy[] strategies)
    {
        ArgumentNullException.ThrowIfNull(strategies);
        if (strategies.Length == 0)
            throw new ArgumentException("Нужна хотя бы одна стратегия скидки.", nameof(strategies));

        _strategies = strategies.ToArray();
    }

    public string Name => string.Join(" + ", _strategies.Select(s => s.Name));

    public decimal CalculateDiscount(decimal subtotal, int nights)
        => _strategies.Sum(strategy => strategy.CalculateDiscount(subtotal, nights));
}
