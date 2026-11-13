namespace HotelsManager;

/// <summary>Тип события бронирования для наблюдателей.</summary>
public enum BookingEventType
{
    Created,
    Confirmed,
    Cancelled
}

/// <summary>Данные события бронирования.</summary>
public sealed class BookingEventArgs : EventArgs
{
    public BookingEventArgs(Booking booking, BookingEventType eventType, string message)
    {
        Booking = booking;
        EventType = eventType;
        Message = message;
    }

    public Booking Booking { get; }
    public BookingEventType EventType { get; }
    public string Message { get; }
}

/// <summary>
/// Наблюдатель за событиями бронирования (паттерн Observer).
/// </summary>
public interface IBookingObserver
{
    void OnBookingEvent(object? sender, BookingEventArgs e);
}
