namespace HotelsManager;

/// <summary>
/// Дополнительный тип уведомления (п. 5 задания) — push-уведомление
/// в мобильном приложении гостя.
/// </summary>
public sealed class PushNotification : INotification
{
    public NotificationType Type => NotificationType.Push;

    public bool Send(string message, string recipient)
    {
        if (string.IsNullOrWhiteSpace(recipient))
            return false;

        Console.WriteLine($"  [PUSH]  {recipient}: {message}");
        return true;
    }
}
