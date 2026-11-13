namespace HotelsManager;

/// <summary>
/// Критерии поиска свободных номеров (п. 4 задания).
/// </summary>
public sealed class RoomFilter
{
    /// <summary>Тип номера; null — любой.</summary>
    public RoomType? Type { get; init; }

    /// <summary>Минимальная вместимость.</summary>
    public int MinCapacity { get; init; } = 1;

    /// <summary>Максимальная цена за ночь; null — без ограничения.</summary>
    public decimal? MaxPricePerNight { get; init; }

    /// <summary>Дата заезда (вместе с <see cref="CheckOut"/>).</summary>
    public DateTime? CheckIn { get; init; }

    /// <summary>Дата выезда (вместе с <see cref="CheckIn"/>).</summary>
    public DateTime? CheckOut { get; init; }

    public bool HasDateRange => CheckIn.HasValue && CheckOut.HasValue;

    public void Validate()
    {
        if (MinCapacity < 1)
            throw new ArgumentOutOfRangeException(nameof(MinCapacity), "Вместимость должна быть не меньше 1.");

        if (MaxPricePerNight is < 0m)
            throw new ArgumentOutOfRangeException(nameof(MaxPricePerNight), "Цена не может быть отрицательной.");

        if (CheckIn.HasValue != CheckOut.HasValue)
            throw new ArgumentException("Нужно указать обе даты: заезда и выезда.");

        if (HasDateRange && CheckOut!.Value.Date <= CheckIn!.Value.Date)
            throw new ArgumentException("Дата выезда должна быть позже даты заезда.");
    }
}
