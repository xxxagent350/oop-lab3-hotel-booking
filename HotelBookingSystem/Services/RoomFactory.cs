namespace HotelsManager;

/// <summary>
/// Фабрика номеров (паттерн Factory): централизованно создаёт номера
/// нужного типа, скрывая конкретные классы от вызывающего кода.
/// </summary>
public sealed class RoomFactory
{
    /// <summary>Создать номер по типу.</summary>
    public IRoom CreateRoom(RoomType type, string roomNumber) => type switch
    {
        RoomType.Standard => CreateStandardRoom(roomNumber),
        RoomType.Deluxe => CreateDeluxeRoom(roomNumber),
        RoomType.Suite => CreateSuite(roomNumber),
        RoomType.Family => CreateFamilyRoom(roomNumber),
        _ => throw new ArgumentOutOfRangeException(nameof(type), type, "Неизвестный тип номера.")
    };

    public IRoom CreateStandardRoom(string roomNumber) => new StandardRoom(roomNumber);

    public IRoom CreateDeluxeRoom(string roomNumber) => new DeluxeRoom(roomNumber);

    public IRoom CreateSuite(string roomNumber) => new Suite(roomNumber);

    public IRoom CreateFamilyRoom(string roomNumber) => new FamilyRoom(roomNumber);
}
