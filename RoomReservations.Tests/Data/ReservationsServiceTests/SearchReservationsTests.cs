using RoomReservations.Data;
using RoomReservations.Models;
using RoomReservations.Tests.Models;

namespace RoomReservations.Tests.Data.ReservationsServiceTests
{
    [TestClass()]
    public class SearchReservationsTests
    {
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

        [TestMethod()]
        public void SearchReservations_NoArgs_ReturnsEverything()
        {
            var date = DateTime.Now;
            _context.Reservations.Add(new Reservation
            {
                StartDate = date.AddDays(-2),
                EndDate = date.AddDays(7),
            });
            _context.Reservations.Add(new Reservation
            {
                StartDate = date,
                EndDate = date.AddDays(10),
            });
            _context.SaveChanges();

            List<Reservation> reservations = _reservationService.SearchReservations();

            Assert.IsNotNull(reservations);
            Assert.AreEqual(2, reservations.Count);
            Assert.AreNotEqual(reservations[0].StartDate, reservations[1].StartDate);
        }

        [TestMethod]
        public void SearchReservations_WithStartDate_ReturnsMatchingReservations()
        {
            // Arrange
            var date = DateTime.Now;
            _context.Reservations.Add(new Reservation
            {
                StartDate = date,
                EndDate = date.AddDays(10),
            });
            _context.Reservations.Add(new Reservation
            {
                StartDate = date,
                EndDate = date.AddDays(7),
            });
            _context.Reservations.Add(new Reservation
            {
                StartDate = date.AddDays(-2),
                EndDate = date.AddDays(7),
            });
            _context.SaveChanges();

            // Act
            var result = _reservationService.SearchReservations(startDate: date);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.All(r => r.StartDate == date));
            Assert.AreEqual(result.Count, 2);
        }

        [TestMethod]
        public void SearchReservations_WithEndDate_ReturnsMatchingReservations()
        {
            // Arrange
            var date = DateTime.Now;
            _context.Reservations.Add(new Reservation
            {
                StartDate = date.AddDays(-2),
                EndDate = date,
            });
            _context.Reservations.Add(new Reservation
            {
                StartDate = date.AddDays(-4),
                EndDate = date.AddDays(-1),
            });
            _context.SaveChanges();

            // Act
            var result = _reservationService.SearchReservations(endDate: date);

            // Assert
            Assert.IsTrue(result.All(r => r.EndDate == date));
            Assert.AreEqual(result.Count, 1);
        }

        [TestMethod]
        public void SearchReservations_WithIsPaid_ReturnsMatchingReservations()
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
            var result = _reservationService.SearchReservations(isPaid: isPaid);

            // Assert
            Assert.IsTrue(result.All(r => r.IsPaid == isPaid));
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(date, result.First().EndDate);
        }

        [TestMethod]
        public void SearchReservations_WithRooms_ReturnsMatchingReservations()
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
            List<Reservation> reservations = [
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

            _context.Reservations.AddRange(reservations); ;
            _context.SaveChanges();
            var roomsFromDb = _context.Rooms.ToList();
            var roomsToFind = new List<Room>
            {
                roomsFromDb[0]
            };

            // Act
            var result = _reservationService.SearchReservations(rooms: roomsToFind);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.All(r => r.RoomReservations.Any(rr => roomsToFind.Contains(rr.Room))));
            Assert.AreEqual(result.Count, 2);
            Assert.AreEqual(result.First().RoomReservations.First().Room.Name, rooms.First().Name);
            Assert.IsTrue(result.All(result => result.StartDate == date.AddDays(-2) || result.StartDate == date.AddDays(1)));
        }
    }
}
