using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RoomReservations.Data;
using RoomReservations.Models;
using RoomReservations.Tests.Models;

namespace RoomReservations.Tests.Data.ReservationsService;

[TestClass]
public class ReservationQueryTests
{
    private readonly DateTime _date = DateTime.Now;


    private readonly List<Room> _rooms =
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

    private readonly ApplicationUser _user = new() { Email = "test@test.com", UserName = "test@test.com" };

    private ApplicationDbContext _context = null!;
    private ReservationService _reservationService = null!;

    private UserManager<ApplicationUser> _userManager = null!;

    [TestInitialize]
    public async Task Initialize()
    {
        _context = TestsContextOptions.TestingContext;
        _reservationService = new ReservationService(_context);

        var userStore = new UserStore<ApplicationUser>(_context);
        _userManager = new UserManager<ApplicationUser>(userStore, null, new PasswordHasher<ApplicationUser>(),
            Array.Empty<IUserValidator<ApplicationUser>>(), Array.Empty<IPasswordValidator<ApplicationUser>>(),
            new UpperInvariantLookupNormalizer(),
            new IdentityErrorDescriber(), null, new Logger<UserManager<ApplicationUser>>(new LoggerFactory()));


        await _userManager.CreateAsync(_user);
    }

    [TestCleanup]
    public void Cleanup()
    {
        _context.Dispose();
    }

    [TestMethod]
    public async Task ReservationQuery_TwoReservations_OneMatchingDateRange()
    {
        List<Reservation> reservations =
        [
            new Reservation
            {
                StartDate = _date.AddDays(1),
                EndDate = _date.AddDays(2),
                RoomReservations =
                [
                    new RoomReservation
                    {
                        Room = _rooms[0]
                    }
                ],
                UserId = _user.Id,
                User = _user
            },

            new Reservation
            {
                StartDate = _date.AddDays(3),
                EndDate = _date.AddDays(5),
                RoomReservations =
                [
                    new RoomReservation
                    {
                        Room = _rooms[1]
                    }
                ],
                UserId = _user.Id,
                User = _user
            }
        ];
        _context.Reservations.AddRange(reservations);
        await _context.SaveChangesAsync();

        var result = await _reservationService.CreateReservationQuery()
            .WhereDatesBetween(_date.AddDays(0), _date.AddDays(2))
            .ToListAsync();

        Assert.IsNotNull(result);
        Assert.AreEqual(result.Count, 1);
        Assert.AreEqual(result[0].StartDate, reservations[0].StartDate);
    }

    [TestMethod]
    public async Task ReservationQuery_TwoReservations_NoneMatchingInDateRange()
    {
        List<Reservation> reservations =
        [
            new Reservation
            {
                StartDate = _date.AddDays(1),
                EndDate = _date.AddDays(2),
                RoomReservations =
                [
                    new RoomReservation
                    {
                        Room = _rooms[0]
                    }
                ],
                UserId = _user.Id,
                User = _user
            },

            new Reservation
            {
                StartDate = _date.AddDays(5),
                EndDate = _date.AddDays(7),
                RoomReservations =
                [
                    new RoomReservation
                    {
                        Room = _rooms[1]
                    }
                ],
                UserId = _user.Id,
                User = _user
            }
        ];
        _context.Reservations.AddRange(reservations);
        await _context.SaveChangesAsync();

        var result = await _reservationService.CreateReservationQuery()
            .WhereDatesBetween(_date.AddDays(3), _date.AddDays(4))
            .ToListAsync();

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
            EndDate = date.AddDays(7),
            UserId = _user.Id,
            User = _user
        });
        _context.Reservations.Add(new Reservation
        {
            StartDate = date,
            EndDate = date.AddDays(10),
            UserId = _user.Id,
            User = _user
        });
        await _context.SaveChangesAsync();

        var reservations = await _reservationService.CreateReservationQuery().ToListAsync();

        Assert.IsNotNull(reservations);
        Assert.AreEqual(2, reservations.Count);
        Assert.AreNotEqual(reservations[0].StartDate, reservations[1].StartDate);
    }

    [TestMethod]
    public async Task ReservationQuery_WhereStartDate_ReturnsTwoMatchingReservations()
    {
        // Arrange
        var date = DateTime.Now;
        _context.Reservations.Add(new Reservation
        {
            StartDate = date,
            EndDate = date.AddDays(10),
            UserId = _user.Id,
            User = _user
        });
        _context.Reservations.Add(new Reservation
        {
            StartDate = date,
            EndDate = date.AddDays(7),
            UserId = _user.Id,
            User = _user
        });
        _context.Reservations.Add(new Reservation
        {
            StartDate = date.AddDays(-2),
            EndDate = date.AddDays(7),
            UserId = _user.Id,
            User = _user
        });
        await _context.SaveChangesAsync();

        // Act
        var result = await _reservationService.CreateReservationQuery()
            .WhereStartDate(date)
            .ToListAsync();

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
            EndDate = date,
            UserId = _user.Id,
            User = _user
        });
        _context.Reservations.Add(new Reservation
        {
            StartDate = date.AddDays(-4),
            EndDate = date.AddDays(-1),
            UserId = _user.Id,
            User = _user
        });
        await _context.SaveChangesAsync();

        // Act
        var result = await _reservationService.CreateReservationQuery()
            .WhereEndDate(date)
            .ToListAsync();

        // Assert
        Assert.IsTrue(result.All(r => r.EndDate == date));
        Assert.AreEqual(result.Count, 1);
    }

    [TestMethod]
    public async Task ReservationQuery_WhereIsPaid_ReturnsMatchingReservation()
    {
        // Arrange
        const bool isPaid = true;
        var date = DateTime.Now;
        _context.Reservations.Add(new Reservation
        {
            StartDate = date.AddDays(-2),
            EndDate = date,
            IsPaid = isPaid,
            UserId = _user.Id,
            User = _user
        });
        _context.Reservations.Add(new Reservation
        {
            StartDate = date.AddDays(-4),
            EndDate = date.AddDays(-1),
            IsPaid = !isPaid,
            UserId = _user.Id,
            User = _user
        });
        await _context.SaveChangesAsync();

        // Act
        var result = await _reservationService.CreateReservationQuery()
            .WhereIsPaid(isPaid)
            .ToListAsync();

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
        await _context.SaveChangesAsync();

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
                ],
                UserId = _user.Id,
                User = _user
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
                ],
                UserId = _user.Id,
                User = _user
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
                ],
                UserId = _user.Id,
                User = _user
            }
        ];

        _context.Reservations.AddRange(reservations);
        await _context.SaveChangesAsync();
        var roomsFromDb = _context.Rooms.ToList();
        var roomsToFind = new List<Room>
        {
            roomsFromDb[0]
        };

        // Act
        var result = await _reservationService.CreateReservationQuery()
            .WhereAnyOfRooms(roomsToFind)
            .WithRooms()
            .ToListAsync();

        // Assert
        Assert.IsNotNull(result);
        Assert.IsTrue(result.All(r => r.RoomReservations.Any(rr => roomsToFind.Contains(rr.Room))));
        Assert.AreEqual(result.Count, 2);
        Assert.AreEqual(result.First().RoomReservations.First().Room.Name, rooms.First().Name);
        Assert.IsTrue(result.All(res => res.StartDate == date.AddDays(-2) || res.StartDate == date.AddDays(1)));
    }

    [TestMethod]
    public async Task ReservationQuery_WhereId_ReturnsMatchingReservation()
    {
        // Arrange
        var date = DateTime.Now;
        const int idToFind = 100;
        _context.Reservations.Add(new Reservation
        {
            Id = idToFind,
            StartDate = date.AddDays(-2),
            EndDate = date,
            UserId = _user.Id,
            User = _user
        });
        _context.Reservations.Add(new Reservation
        {
            Id = idToFind + 1,
            StartDate = date.AddDays(-4),
            EndDate = date.AddDays(-1),
            UserId = _user.Id,
            User = _user
        });
        _context.Reservations.Add(new Reservation
        {
            Id = idToFind - 1,
            StartDate = date.AddDays(-40),
            EndDate = date.AddDays(-10),
            UserId = _user.Id,
            User = _user
        });
        await _context.SaveChangesAsync();

        // Act
        var result = await _reservationService.CreateReservationQuery()
            .WhereId(idToFind)
            .ToListAsync();

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
        const int idToFind = 100;
        _context.Reservations.Add(new Reservation
        {
            Id = idToFind - 1,
            StartDate = date.AddDays(-2),
            EndDate = date,
            UserId = _user.Id,
            User = _user
        });
        _context.Reservations.Add(new Reservation
        {
            Id = idToFind + 1,
            StartDate = date.AddDays(-4),
            EndDate = date.AddDays(-1),
            UserId = _user.Id,
            User = _user
        });
        await _context.SaveChangesAsync();

        // Act
        var result = await _reservationService.CreateReservationQuery()
            .WhereId(idToFind)
            .ToListAsync();

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(result.Count, 0);
    }
}