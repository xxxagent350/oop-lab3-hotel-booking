using System.Text;

namespace HotelsManager;

internal static class Program
{
    private static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;

        Console.WriteLine("==============================================================");
        Console.WriteLine("            СИСТЕМА БРОНИРОВАНИЯ ОТЕЛЯ");
        Console.WriteLine("            Паттерны: Factory, Strategy, Observer");
        Console.WriteLine("==============================================================");

        // Наблюдатели (каналы уведомлений).
        var notifications = new NotificationManager();
        notifications.AddNotification(new EmailNotification());
        notifications.AddNotification(new SMSNotification());
        notifications.AddNotification(new PushNotification());

        var hotel = new HotelManager(notifications);
        var factory = new RoomFactory();

        DemoFactory(hotel, factory);
        DemoStrategies();
        DemoNotifications(notifications);
        DemoBooking(hotel);
        DemoStatistics(hotel);
    }

    /* ===================== 1. Factory ===================== */

    private static void DemoFactory(HotelManager hotel, RoomFactory factory)
    {
        Section("1. ТИПЫ НОМЕРОВ (паттерн Factory)");

        foreach (RoomType type in Enum.GetValues<RoomType>())
        {
            IRoom room = factory.CreateRoom(type, RoomNumberFor(type));
            hotel.AddRoom(room);

            Console.WriteLine($"  {room.GetDescription()}");
            Console.WriteLine($"     базовая цена: {room.BasePrice:N0} руб./ночь | вместимость: {room.Capacity} гост.");
        }
    }

    private static string RoomNumberFor(RoomType type) => type switch
    {
        RoomType.Standard => "101",
        RoomType.Deluxe => "201",
        RoomType.Suite => "301",
        RoomType.Family => "401",
        _ => throw new ArgumentOutOfRangeException(nameof(type))
    };

    /* ===================== 2. Strategy ===================== */

    private static void DemoStrategies()
    {
        Section("2. РАСЧЁТ СТОИМОСТИ И СКИДКИ (паттерн Strategy)");

        const decimal basePrice = 2500m;
        const int nights = 10;
        var checkIn = new DateTime(2026, 12, 24);

        Console.WriteLine($"  Базовая цена: {basePrice:N0} руб./ночь | Ночей: {nights} | Заезд: {checkIn:dd.MM.yyyy}");
        Console.WriteLine();

        IPriceStrategy[] priceStrategies =
        {
            new RegularPriceStrategy(),
            new WeekendPriceStrategy(),
            new SeasonalPriceStrategy()
        };

        decimal seasonalSubtotal = 0m;
        foreach (var strategy in priceStrategies)
        {
            decimal total = strategy.CalculatePrice(basePrice, nights, checkIn);
            if (strategy is SeasonalPriceStrategy)
                seasonalSubtotal = total;

            Console.WriteLine($"  {strategy.Name,-32}{total,14:N0} руб.");
        }

        Console.WriteLine();
        Console.WriteLine("  Система скидок (применяется к сезонному тарифу):");

        IDiscountStrategy[] discounts =
        {
            new LongStayDiscountStrategy(),
            new LoyaltyDiscountStrategy(),
            new CompositeDiscountStrategy(new LongStayDiscountStrategy(), new LoyaltyDiscountStrategy())
        };

        foreach (var discount in discounts)
        {
            decimal amount = discount.CalculateDiscount(seasonalSubtotal, nights);
            Console.WriteLine($"  {discount.Name}");
            Console.WriteLine($"    скидка -{amount:N0} руб. → итого {seasonalSubtotal - amount:N0} руб.");
        }
    }

    /* ===================== 3. Уведомления ===================== */

    private static void DemoNotifications(NotificationManager notifications)
    {
        Section("3. УВЕДОМЛЕНИЯ (дополнительный канал Push)");

        Console.WriteLine("  Тестовая рассылка по всем каналам:");
        notifications.Broadcast(
            "Добро пожаловать! Ваше бронирование ожидает подтверждения.",
            "client@example.com");
    }

    /* ===================== 4. Поиск и бронирование ===================== */

    private static void DemoBooking(HotelManager hotel)
    {
        Section("4. ПОИСК СВОБОДНЫХ НОМЕРОВ И БРОНИРОВАНИЕ");

        var checkIn = new DateTime(2026, 12, 25);
        var checkOut = new DateTime(2027, 1, 5); // 11 ночей

        var filter = new RoomFilter
        {
            Type = RoomType.Deluxe,
            MinCapacity = 2,
            MaxPricePerNight = 5000m,
            CheckIn = checkIn,
            CheckOut = checkOut
        };

        Console.WriteLine(
            $"  Критерии: тип Deluxe, вместимость от {filter.MinCapacity} гост., " +
            $"цена до {filter.MaxPricePerNight!.Value:N0} руб., {checkIn:dd.MM.yyyy}–{checkOut:dd.MM.yyyy}");

        var available = hotel.FindAvailableRooms(filter);
        Console.WriteLine($"  Найдено номеров: {available.Count}");

        foreach (var room in available)
            Console.WriteLine($"    • {room.GetDescription()} — {room.BasePrice:N0} руб./ночь");

        if (available.Count == 0)
        {
            Console.WriteLine("  Подходящих номеров нет.");
            return;
        }

        var discount = new CompositeDiscountStrategy(
            new LongStayDiscountStrategy(),
            new LoyaltyDiscountStrategy());

        var booking = hotel.CreateBooking(
            available[0].RoomNumber,
            "Иван Петров",
            "ivan.petrov@mail.ru",
            checkIn,
            checkOut,
            new SeasonalPriceStrategy(),
            discount);

        Console.WriteLine();
        Console.WriteLine($"  Создано бронирование #{booking.BookingId}: {booking.Room.GetDescription()}");
        Console.WriteLine($"  Гость: {booking.GuestName} | {booking.CheckIn:dd.MM.yyyy}–{booking.CheckOut:dd.MM.yyyy} | ночей: {booking.Nights}");
        Console.WriteLine($"  Тариф: {booking.PriceStrategy.Name}");
        Console.WriteLine(
            $"  Стоимость: {booking.Subtotal:N0} руб. − скидка {booking.DiscountAmount:N0} руб. = {booking.TotalPrice:N0} руб.");

        Console.WriteLine();
        Console.WriteLine("  Подтверждение бронирования (Observer рассылает уведомления):");
        booking.Confirm();

        var afterConfirm = hotel.FindAvailableRooms(filter);
        Console.WriteLine(
            $"  После подтверждения свободных Deluxe-номеров на эти даты: {afterConfirm.Count} " +
            $"(номер {booking.Room.RoomNumber} больше не предлагается)");

        Console.WriteLine();
        Console.WriteLine("  Второе бронирование и его отмена:");

        IRoom standardRoom = hotel.FindAvailableRooms(RoomType.Standard).First();
        var second = hotel.CreateBooking(
            standardRoom.RoomNumber,
            "Мария Иванова",
            "maria.ivanova@example.com",
            new DateTime(2027, 2, 1),
            new DateTime(2027, 2, 6),
            new RegularPriceStrategy());

        second.Confirm();
        second.Cancel();
    }

    /* ===================== 5. Статистика ===================== */

    private static void DemoStatistics(HotelManager hotel)
    {
        Section("5. СТАТУС ОТЕЛЯ И СТАТИСТИКА");

        hotel.ShowHotelStatus();

        Console.WriteLine();
        Console.WriteLine("  Статистика по типам номеров:");
        foreach (var pair in hotel.GetRoomStatistics())
            Console.WriteLine($"    {pair.Key,-10} {pair.Value} номер(ов)");

        Console.WriteLine();
        Console.WriteLine("=== ДЕМОНСТРАЦИЯ ЗАВЕРШЕНА ===");
    }

    private static void Section(string title)
    {
        Console.WriteLine();
        Console.WriteLine(new string('=', 62));
        Console.WriteLine(title);
        Console.WriteLine(new string('=', 62));
    }
}
