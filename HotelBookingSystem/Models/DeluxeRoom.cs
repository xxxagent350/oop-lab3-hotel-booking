namespace HotelsManager;

/// <summary>Номер «Делюкс» с видом на море.</summary>
public class DeluxeRoom : RoomBase
{
    public DeluxeRoom(string roomNumber) : base(roomNumber)
    {
    }

    public override RoomType Type => RoomType.Deluxe;
    public override decimal BasePrice => 4500m;
    public override int Capacity => 3;

    public override string GetDescription()
        => $"Делюкс номер {RoomNumber} с видом на море";
}
