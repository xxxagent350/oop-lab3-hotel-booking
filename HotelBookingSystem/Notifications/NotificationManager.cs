namespace HotelsManager;

/// <summary>
/// Менеджер каналов уведомлений. Умеет рассылать сообщения напрямую,
/// а также подписывать все каналы на события конкретного бронирования
/// (паттерн Observer).
/// </summary>
public sealed class NotificationManager
{
    private readonly List<INotification> _notifications = new();

    public IReadOnlyList<INotification> Notifications => _notifications;

    public void AddNotification(INotification notification)
    {
        ArgumentNullException.ThrowIfNull(notification);

        if (!_notifications.Contains(notification))
            _notifications.Add(notification);
    }

    public bool RemoveNotification(INotification notification)
        => _notifications.Remove(notification);

    /// <summary>Отправить сообщение по всем подключённым каналам.</summary>
    public void Broadcast(string message, string recipient)
    {
        foreach (var notification in _notifications)
            notification.Send(message, recipient);
    }

    /// <summary>
    /// Подписать все каналы на события бронирования. Для каждого канала
    /// создаётся Observer-адаптер.
    /// </summary>
    public void Subscribe(Booking booking)
    {
        ArgumentNullException.ThrowIfNull(booking);

        foreach (var notification in _notifications)
            booking.Subscribe(new NotificationObserver(notification));
    }
}
