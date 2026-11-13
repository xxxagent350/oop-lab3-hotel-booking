namespace HotelsManager;

/// <summary>Канал уведомления (паттерн Observer: конкретный наблюдатель).</summary>
public interface INotification
{
    bool Send(string message, string recipient);
    NotificationType Type { get; }
}
