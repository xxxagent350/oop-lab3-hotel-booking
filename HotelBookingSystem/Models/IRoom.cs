namespace HotelsManager;

/// <summary>Общий контракт гостиничного номера.</summary>
public interface IRoom
{
    string RoomNumber { get; }
    RoomType Type { get; }
    decimal BasePrice { get; }
    int Capacity { get; }
    bool IsAvailable { get; set; }
    string GetDescription();
}
