namespace HotelsManager;

/// <summary>
/// Дополнительный тип номера (п. 5 задания): просторный семейный номер
/// с двумя спальнями и кухонной зоной.
/// </summary>
public class FamilyRoom : RoomBase
{
    public FamilyRoom(string roomNumber) : base(roomNumber)
    {
    }

    public override RoomType Type => RoomType.Family;
    public override decimal BasePrice => 5500m;
    public override int Capacity => 5;

    public override string GetDescription()
        => $"Семейный номер {RoomNumber} с двумя спальнями и кухонной зоной";
}
