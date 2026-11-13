namespace HotelsManager;

/// <summary>
/// Адаптер Observer → уведомление: получает событие бронирования и передаёт
/// текст сообщения в конкретный канал (<see cref="INotification"/>).
/// </summary>
public sealed class NotificationObserver : IBookingObserver
{
    private readonly INotification _notification;

    public NotificationObserver(INotification notification)
    {
        _notification = notification ?? throw new ArgumentNullException(nameof(notification));
    }

    public void OnBookingEvent(object? sender, BookingEventArgs e)
        => _notification.Send(e.Message, e.Booking.GuestEmail);
}
