namespace HotelsManager;

/// <summary>
/// Бронирование номера.
/// Паттерн Observer: <see cref="Booking"/> — субъект, <see cref="IBookingObserver"/> —
/// наблюдатели. При создании, подтверждении и отмене бронирования субъект
/// оповещает всех подписчиков.
/// </summary>
public sealed class Booking
{
    private static int _nextId = 1;
    private readonly List<IBookingObserver> _observers = new();

    public Booking(
        IRoom room,
        string guestName,
        string guestEmail,
        DateTime checkIn,
        DateTime checkOut,
        IPriceStrategy priceStrategy,
        IDiscountStrategy? discountStrategy = null)
    {
        Room = room ?? throw new ArgumentNullException(nameof(room));

        if (string.IsNullOrWhiteSpace(guestName))
            throw new ArgumentException("Имя гостя не может быть пустым.", nameof(guestName));
        if (string.IsNullOrWhiteSpace(guestEmail))
            throw new ArgumentException("E-mail гостя не может быть пустым.", nameof(guestEmail));

        CheckIn = checkIn.Date;
        CheckOut = checkOut.Date;
        if (CheckOut <= CheckIn)
            throw new ArgumentException("Дата выезда должна быть позже даты заезда.");

        GuestName = guestName.Trim();
        GuestEmail = guestEmail.Trim();
        PriceStrategy = priceStrategy ?? throw new ArgumentNullException(nameof(priceStrategy));
        DiscountStrategy = discountStrategy ?? NoDiscountStrategy.Instance;

        BookingId = _nextId++;
        Nights = (int)(CheckOut - CheckIn).TotalDays;

        Subtotal = PriceStrategy.CalculatePrice(Room.BasePrice, Nights, CheckIn);
        DiscountAmount = Math.Clamp(DiscountStrategy.CalculateDiscount(Subtotal, Nights), 0m, Subtotal);
        TotalPrice = Subtotal - DiscountAmount;
    }

    public int BookingId { get; }
    public IRoom Room { get; }
    public string GuestName { get; }
    public string GuestEmail { get; }
    public DateTime CheckIn { get; }
    public DateTime CheckOut { get; }
    public int Nights { get; }
    public decimal Subtotal { get; }
    public decimal DiscountAmount { get; }
    public decimal TotalPrice { get; }
    public BookingStatus Status { get; private set; } = BookingStatus.Pending;
    public IPriceStrategy PriceStrategy { get; }
    public IDiscountStrategy DiscountStrategy { get; }

    /// <summary>Статус бронирования по-русски — для вывода пользователю.</summary>
    public string StatusText => Status switch
    {
        BookingStatus.Pending => "ожидает подтверждения",
        BookingStatus.Confirmed => "подтверждено",
        BookingStatus.Cancelled => "отменено",
        _ => Status.ToString()
    };

    /// <summary>Подписать наблюдателя на события бронирования (Observer).</summary>
    public void Subscribe(IBookingObserver observer)
    {
        ArgumentNullException.ThrowIfNull(observer);

        if (!_observers.Contains(observer))
            _observers.Add(observer);
    }

    /// <summary>Отписать наблюдателя.</summary>
    public bool Unsubscribe(IBookingObserver observer)
        => _observers.Remove(observer);

    /// <summary>Сообщить подписчикам о создании бронирования.</summary>
    public void RaiseCreated()
        => Notify(
            BookingEventType.Created,
            $"Бронирование #{BookingId} создано: {Room.GetDescription()}, " +
            $"{CheckIn:dd.MM.yyyy}–{CheckOut:dd.MM.yyyy}, {Nights} ноч., к оплате {TotalPrice:N0} руб.");

    public void Confirm()
    {
        if (Status == BookingStatus.Cancelled)
            throw new InvalidOperationException("Нельзя подтвердить отменённое бронирование.");
        if (Status == BookingStatus.Confirmed)
            return;

        Status = BookingStatus.Confirmed;
        Room.IsAvailable = false;

        Notify(
            BookingEventType.Confirmed,
            $"Бронирование #{BookingId} подтверждено: {Room.GetDescription()}, " +
            $"{CheckIn:dd.MM.yyyy}–{CheckOut:dd.MM.yyyy}. Сумма: {TotalPrice:N0} руб.");
    }

    public void Cancel()
    {
        if (Status == BookingStatus.Cancelled)
            return;

        Status = BookingStatus.Cancelled;
        Room.IsAvailable = true;

        Notify(
            BookingEventType.Cancelled,
            $"Бронирование #{BookingId} отменено: {Room.GetDescription()}, " +
            $"{CheckIn:dd.MM.yyyy}–{CheckOut:dd.MM.yyyy}.");
    }

    private void Notify(BookingEventType eventType, string message)
    {
        var args = new BookingEventArgs(this, eventType, message);

        foreach (var observer in _observers.ToArray())
            observer.OnBookingEvent(this, args);
    }

    public override string ToString()
        => $"#{BookingId} {GuestName} — {Room.GetDescription()} ({CheckIn:dd.MM.yyyy}–{CheckOut:dd.MM.yyyy}), " +
           $"{StatusText}, {TotalPrice:N0} руб.";
}
