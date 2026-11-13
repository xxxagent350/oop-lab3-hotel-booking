namespace HotelsManager;

/// <summary>Уведомление по SMS.</summary>
public sealed class SMSNotification : INotification
{
    public NotificationType Type => NotificationType.SMS;

    public bool Send(string message, string recipient)
    {
        if (string.IsNullOrWhiteSpace(recipient))
            return false;

        Console.WriteLine($"  [SMS]   {recipient}: {message}");
        return true;
    }
}
