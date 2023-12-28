using RoomReservations.Data;
using RoomReservations.Models;
using RoomReservations.Tests.Models;

namespace RoomReservations.Tests.Data.ReservationsService;

[TestClass]
public class ReservationQueryTests
{
    private readonly DateTime _date = new(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    private readonly List<Reservation> reservations = [];

    private readonly List<Room> rooms =
    [
        new Room
        {
            Name = "Test Room 1",
            PricePerNight = 30.50M
        },
        new Room
        {
            Name = "Test Room 2",
            PricePerNight = 130.99M
        },
        new Room
        {
            Name = "Test Room 3",
            PricePerNight = 99.99M
        }
    ];

    private ApplicationDbContext _context = null!;
    private ReservationService _reservationService = null!;

    [TestInitialize]
    public void Initialize()
    {
        _context = TestsContextOptions.TestingContext;
        _reservationService = new ReservationService(_context);
    }

    [TestCleanup]
    public void Cleanup()
    {
        _context.Dispose();
    }

    [TestMethod]
    public async Task ReservationQuery_TwoReservations_OneMatchingDateRange()
    {
        reservations.Add(new Reservation
        {
            StartDate = _date.AddDays(1),
            EndDate = _date.AddDays(2),
            RoomReservations =
            [
                new RoomReservation
                {
                    Room = rooms[0]
                }
            ]
        });
        reservations.Add(new Reservation
        {
            StartDate = _date.AddDays(3),
            EndDate = _date.AddDays(5),
            RoomReservations =
            [
                new RoomReservation
                {
                    Room = rooms[1]
                }
            ]
        });
        _context.Reservations.AddRange(reservations);
        await _context.SaveChangesAsync();

        var result = await _reservationService.CreateReservationQuery()
            .WhereDatesBetween(_date.AddDays(0), _date.AddDays(2))
            .ExecuteAsync();

        Assert.IsNotNull(result);
        Assert.AreEqual(result.Count, 1);
        Assert.AreEqual(result[0].StartDate, reservations[0].StartDate);
    }

    [TestMethod]
    public async Task ReservationQuery_TwoReservations_NoneMatchingInDateRange()
    {
        reservations.Add(new Reservation
        {
            StartDate = _date.AddDays(1),
            EndDate = _date.AddDays(2),
            RoomReservations =
            [
                new RoomReservation
                {
                    Room = rooms[0]
                }
            ]
        });
        reservations.Add(new Reservation
        {
            StartDate = _date.AddDays(5),
            EndDate = _date.AddDays(7),
            RoomReservations =
            [
                new RoomReservation
                {
                    Room = rooms[1]
                }
            ]
        });
        _context.Reservations.AddRange(reservations);
        await _context.SaveChangesAsync();

        var result = await _reservationService.CreateReservationQuery()
            .WhereDatesBetween(_date.AddDays(3), _date.AddDays(4))
            .ExecuteAsync();
        ;

        Assert.IsNotNull(result);
        Assert.AreEqual(result.Count, 0);
    }

    [TestMethod]
    public async Task ReservationQuery_NoArgs_ReturnsEverything()
    {
        var date = DateTime.Now;
        _context.Reservations.Add(new Reservation
        {
            StartDate = date.AddDays(-2),
            EndDate = date.AddDays(7)
        });
        _context.Reservations.Add(new Reservation
        {
            StartDate = date,
            EndDate = date.AddDays(10)
        });
        _context.SaveChanges();

        var reservations = await _reservationService.CreateReservationQuery().ExecuteAsync();

        Assert.IsNotNull(reservations);
        Assert.AreEqual(2, reservations.Count);
        Assert.AreNotEqual(reservations[0].StartDate, reservations[1].StartDate);
    }

    [TestMethod]
    public async Task ReservationQuery_WhereStartDatee_ReturnsTwoMatchingReservations()
    {
        // Arrange
        var date = DateTime.Now;
        _context.Reservations.Add(new Reservation
        {
            StartDate = date,
            EndDate = date.AddDays(10)
        });
        _context.Reservations.Add(new Reservation
        {
            StartDate = date,
            EndDate = date.AddDays(7)
        });
        _context.Reservations.Add(new Reservation
        {
            StartDate = date.AddDays(-2),
            EndDate = date.AddDays(7)
        });
        _context.SaveChanges();

        // Act
        var result = await _reservationService.CreateReservationQuery()
            .WhereStartDate(date)
            .ExecuteAsync();

        // Assert
        Assert.IsNotNull(result);
        Assert.IsTrue(result.All(r => r.StartDate == date));
        Assert.AreEqual(result.Count, 2);
    }

    [TestMethod]
    public async Task ReservationQuery_WhereEndDate_ReturnsMatchingReservation()
    {
        // Arrange
        var date = DateTime.Now;
        _context.Reservations.Add(new Reservation
        {
            StartDate = date.AddDays(-2),
            EndDate = date
        });
        _context.Reservations.Add(new Reservation
        {
            StartDate = date.AddDays(-4),
            EndDate = date.AddDays(-1)
        });
        _context.SaveChanges();

        // Act
        var result = await _reservationService.CreateReservationQuery()
            .WhereEndDate(date)
            .ExecuteAsync();

        // Assert
        Assert.IsTrue(result.All(r => r.EndDate == date));
        Assert.AreEqual(result.Count, 1);
    }

    [TestMethod]
    public async Task ReservationQuery_WhereIsPaid_ReturnsMatchingReservation()
    {
        // Arrange
        var isPaid = true;
        var date = DateTime.Now;
        _context.Reservations.Add(new Reservation
        {
            StartDate = date.AddDays(-2),
            EndDate = date,
            IsPaid = isPaid
        });
        _context.Reservations.Add(new Reservation
        {
            StartDate = date.AddDays(-4),
            EndDate = date.AddDays(-1),
            IsPaid = !isPaid
        });
        _context.SaveChanges();

        // Act
        var result = await _reservationService.CreateReservationQuery()
            .WhereIsPaid(isPaid)
            .ExecuteAsync();

        // Assert
        Assert.IsTrue(result.All(r => r.IsPaid == isPaid));
        Assert.AreEqual(1, result.Count);
        Assert.AreEqual(date, result.First().EndDate);
    }

    [TestMethod]
    public async Task ReservationQuery_WhereAnyOfRooms_ReturnsTwoMatchingReservationsWithTheirRooms()
    {
        // Arrange
        List<Room> rooms =
        [
            new Room
            {
                Name = "Test Room 1",
                PricePerNight = 30.50M
            },
            new Room
            {
                Name = "Test Room 2",
                PricePerNight = 130.99M
            }
        ];
        _context.Rooms.AddRange(rooms);
        _context.SaveChanges();

        var date = DateTime.Now;
        List<Reservation> reservations =
        [
            new Reservation
            {
                StartDate = date.AddDays(-2),
                EndDate = date,
                RoomReservations =
                [
                    new RoomReservation
                    {
                        Room = _context.Rooms.First(r => r.Name == rooms[0].Name)
                    }
                ]
            },
            new Reservation
            {
                StartDate = date.AddDays(1),
                EndDate = date.AddDays(4),
                RoomReservations =
                [
                    new RoomReservation
                    {
                        Room = _context.Rooms.First(r => r.Name == rooms[0].Name)
                    },
                    new RoomReservation
                    {
                        Room = _context.Rooms.First(r => r.Name == rooms[1].Name)
                    }
                ]
            },
            new Reservation
            {
                StartDate = date.AddDays(4),
                EndDate = date.AddDays(10),
                RoomReservations =
                [
                    new RoomReservation
                    {
                        Room = _context.Rooms.First(r => r.Name == rooms[1].Name)
                    }
                ]
            }
        ];

        _context.Reservations.AddRange(reservations);
        _context.SaveChanges();
        var roomsFromDb = _context.Rooms.ToList();
        var roomsToFind = new List<Room>
        {
            roomsFromDb[0]
        };

        // Act
        var result = await _reservationService.CreateReservationQuery()
            .WhereAnyOfRooms(roomsToFind)
            .WithRooms()
            .ExecuteAsync();

        // Assert
        Assert.IsNotNull(result);
        Assert.IsTrue(result.All(r => r.RoomReservations.Any(rr => roomsToFind.Contains(rr.Room))));
        Assert.AreEqual(result.Count, 2);
        Assert.AreEqual(result.First().RoomReservations.First().Room.Name, rooms.First().Name);
        Assert.IsTrue(result.All(result =>
            result.StartDate == date.AddDays(-2) || result.StartDate == date.AddDays(1)));
    }

    [TestMethod]
    public async Task ReservationQuery_WhereId_ReturnsMatchingReservation()
    {
        // Arrange
        var date = DateTime.Now;
        var idToFind = 100;
        _context.Reservations.Add(new Reservation
        {
            Id = idToFind,
            StartDate = date.AddDays(-2),
            EndDate = date
        });
        _context.Reservations.Add(new Reservation
        {
            Id = idToFind + 1,
            StartDate = date.AddDays(-4),
            EndDate = date.AddDays(-1)
        });
        _context.Reservations.Add(new Reservation
        {
            Id = idToFind - 1,
            StartDate = date.AddDays(-40),
            EndDate = date.AddDays(-10)
        });
        _context.SaveChanges();

        // Act
        var result = await _reservationService.CreateReservationQuery()
            .WhereId(idToFind)
            .ExecuteAsync();

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(result.Count, 1);
        Assert.AreEqual(result.First().Id, idToFind);
        Assert.AreEqual(result.First().EndDate, date);
    }

    [TestMethod]
    public async Task ReservationQuery_WhereId_ReturnsEmpty()
    {
        // Arrange
        var date = DateTime.Now;
        var idToFind = 100;
        _context.Reservations.Add(new Reservation
        {
            Id = idToFind - 1,
            StartDate = date.AddDays(-2),
            EndDate = date
        });
        _context.Reservations.Add(new Reservation
        {
            Id = idToFind + 1,
            StartDate = date.AddDays(-4),
            EndDate = date.AddDays(-1)
        });
        _context.SaveChanges();

        // Act
        var result = await _reservationService.CreateReservationQuery()
            .WhereId(idToFind)
            .ExecuteAsync();

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(result.Count, 0);
    }
}