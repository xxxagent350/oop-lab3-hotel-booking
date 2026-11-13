namespace HotelsManager;

/// <summary>Отсутствие скидки.</summary>
public sealed class NoDiscountStrategy : IDiscountStrategy
{
    public static NoDiscountStrategy Instance { get; } = new();

    private NoDiscountStrategy()
    {
    }

    public string Name => "Без скидки";

    public decimal CalculateDiscount(decimal subtotal, int nights) => 0m;
}
