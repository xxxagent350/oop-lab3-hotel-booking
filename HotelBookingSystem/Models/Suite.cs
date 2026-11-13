namespace HotelsManager;

/// <summary>Номер «Люкс» с гостиной и джакузи.</summary>
public class Suite : RoomBase
{
    public Suite(string roomNumber) : base(roomNumber)
    {
    }

    public override RoomType Type => RoomType.Suite;
    public override decimal BasePrice => 7500m;
    public override int Capacity => 4;

    public override string GetDescription()
        => $"Люкс номер {RoomNumber} с гостиной и джакузи";
}
