namespace HotelsManager;

/// <summary>
/// Управление отелем: реестр номеров, подбор свободных номеров,
/// создание и ведение бронирований, статистика.
/// </summary>
public sealed class HotelManager
{
    private readonly List<IRoom> _rooms = new();
    private readonly List<Booking> _bookings = new();
    private readonly NotificationManager _notificationManager;

    public HotelManager(NotificationManager notificationManager)
    {
        _notificationManager = notificationManager ?? throw new ArgumentNullException(nameof(notificationManager));
    }

    public IReadOnlyList<IRoom> Rooms => _rooms;
    public IReadOnlyList<Booking> Bookings => _bookings;

    public void AddRoom(IRoom room)
    {
        ArgumentNullException.ThrowIfNull(room);

        if (_rooms.Any(existing => string.Equals(existing.RoomNumber, room.RoomNumber, StringComparison.OrdinalIgnoreCase)))
            throw new InvalidOperationException($"Номер {room.RoomNumber} уже добавлен в отель.");

        _rooms.Add(room);
    }

    public void AddRooms(IEnumerable<IRoom> rooms)
    {
        ArgumentNullException.ThrowIfNull(rooms);

        foreach (var room in rooms)
            AddRoom(room);
    }

    public IRoom? FindRoom(string roomNumber)
    {
        if (string.IsNullOrWhiteSpace(roomNumber))
            return null;

        string normalized = roomNumber.Trim();
        return _rooms.FirstOrDefault(room =>
            string.Equals(room.RoomNumber, normalized, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>Все номера, свободные в данный момент.</summary>
    public List<IRoom> FindAvailableRooms()
        => _rooms.Where(room => room.IsAvailable).ToList();

    /// <summary>Свободные номера заданного типа.</summary>
    public List<IRoom> FindAvailableRooms(RoomType roomType)
        => _rooms.Where(room => room.IsAvailable && room.Type == roomType).ToList();

    /// <summary>
    /// Поиск свободных номеров по набору критериев: тип, вместимость,
    /// максимальная цена и период проживания (п. 4 задания).
    /// </summary>
    public List<IRoom> FindAvailableRooms(RoomFilter filter)
    {
        ArgumentNullException.ThrowIfNull(filter);
        filter.Validate();

        IEnumerable<IRoom> query = _rooms;

        if (filter.Type.HasValue)
            query = query.Where(room => room.Type == filter.Type.Value);

        query = query.Where(room => room.Capacity >= filter.MinCapacity);

        if (filter.MaxPricePerNight.HasValue)
            query = query.Where(room => room.BasePrice <= filter.MaxPricePerNight.Value);

        if (filter.HasDateRange)
            query = query.Where(room => IsRoomFreeForDates(room, filter.CheckIn!.Value, filter.CheckOut!.Value));
        else
            query = query.Where(room => room.IsAvailable);

        return query
            .OrderBy(room => room.BasePrice)
            .ThenBy(room => room.RoomNumber)
            .ToList();
    }

    /// <summary>Свободен ли номер на указанный период (пересечение броней не допускается).</summary>
    public bool IsRoomFreeForDates(IRoom room, DateTime checkIn, DateTime checkOut)
    {
        ArgumentNullException.ThrowIfNull(room);

        DateTime from = checkIn.Date;
        DateTime to = checkOut.Date;

        if (to <= from)
            throw new ArgumentException("Дата выезда должна быть позже даты заезда.");

        return !_bookings.Any(booking =>
            booking.Status != BookingStatus.Cancelled &&
            string.Equals(booking.Room.RoomNumber, room.RoomNumber, StringComparison.OrdinalIgnoreCase) &&
            booking.CheckIn < to &&
            from < booking.CheckOut);
    }

    /// <summary>Создать бронирование и подписать на его события все каналы уведомлений.</summary>
    public Booking CreateBooking(
        string roomNumber,
        string guestName,
        string guestEmail,
        DateTime checkIn,
        DateTime checkOut,
        IPriceStrategy priceStrategy,
        IDiscountStrategy? discountStrategy = null)
    {
        IRoom room = FindRoom(roomNumber)
            ?? throw new ArgumentException($"Номер {roomNumber} не найден.", nameof(roomNumber));

        if (!IsRoomFreeForDates(room, checkIn, checkOut))
            throw new InvalidOperationException($"Номер {roomNumber} уже забронирован на выбранные даты.");

        var booking = new Booking(room, guestName, guestEmail, checkIn, checkOut, priceStrategy, discountStrategy);

        _notificationManager.Subscribe(booking);
        _bookings.Add(booking);
        booking.RaiseCreated();

        return booking;
    }

    public Booking GetBooking(int bookingId)
        => _bookings.FirstOrDefault(booking => booking.BookingId == bookingId)
           ?? throw new ArgumentException($"Бронирование #{bookingId} не найдено.", nameof(bookingId));

    public void ConfirmBooking(int bookingId) => GetBooking(bookingId).Confirm();

    public void CancelBooking(int bookingId) => GetBooking(bookingId).Cancel();

    /// <summary>Статистика по типам номеров (п. 4 задания).</summary>
    public Dictionary<RoomType, int> GetRoomStatistics()
    {
        var statistics = Enum.GetValues<RoomType>().ToDictionary(type => type, _ => 0);

        foreach (var group in _rooms.GroupBy(room => room.Type))
            statistics[group.Key] = group.Count();

        return statistics;
    }

    /// <summary>Вывести текущее состояние отеля в консоль.</summary>
    public void ShowHotelStatus()
    {
        Console.WriteLine($"  Всего номеров: {_rooms.Count}; свободно сейчас: {_rooms.Count(room => room.IsAvailable)}");

        foreach (var room in _rooms.OrderBy(room => room.RoomNumber))
        {
            string state = room.IsAvailable ? "свободен" : "занят";
            Console.WriteLine($"    {room.GetDescription()} — {room.BasePrice:N0} руб./ночь, до {room.Capacity} гост., {state}");
        }

        int active = _bookings.Count(booking => booking.Status != BookingStatus.Cancelled);
        Console.WriteLine($"  Бронирований: {_bookings.Count}; активных: {active}");

        foreach (var booking in _bookings)
            Console.WriteLine($"    {booking}");
    }
}
