namespace HotelsManager;

/// <summary>Базовая реализация номера с общей для всех типов логикой.</summary>
public abstract class RoomBase : IRoom
{
    protected RoomBase(string roomNumber)
    {
        if (string.IsNullOrWhiteSpace(roomNumber))
            throw new ArgumentException("Номер комнаты не может быть пустым.", nameof(roomNumber));

        RoomNumber = roomNumber.Trim();
    }

    public string RoomNumber { get; }
    public abstract RoomType Type { get; }
    public abstract decimal BasePrice { get; }
    public abstract int Capacity { get; }
    public bool IsAvailable { get; set; } = true;

    public abstract string GetDescription();

    /// <summary>Стоимость проживания по базовой цене без тарифов и скидок.</summary>
    public virtual decimal CalculateTotalPrice(int days)
    {
        if (days <= 0)
            throw new ArgumentOutOfRangeException(nameof(days), "Количество суток должно быть положительным.");

        return BasePrice * days;
    }

    public override string ToString()
        => $"{GetDescription()} — {BasePrice:N0} руб./ночь, до {Capacity} гост.";
}
