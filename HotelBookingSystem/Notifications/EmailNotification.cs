namespace HotelsManager;

/// <summary>Уведомление по электронной почте.</summary>
public sealed class EmailNotification : INotification
{
    public NotificationType Type => NotificationType.Email;

    public bool Send(string message, string recipient)
    {
        if (string.IsNullOrWhiteSpace(recipient))
            return false;

        Console.WriteLine($"  [EMAIL] {recipient}: {message}");
        return true;
    }
}
