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
            _context.Reservations.Add(new Reservation
            {
                StartDate = DateTime.Now.AddDays(-2),
                EndDate = DateTime.Now.AddDays(7),
            });
            _context.Reservations.Add(new Reservation
            {
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(10),
            });
            _context.SaveChanges();

            List<Reservation> reservations = _reservationService.SearchReservations();

            Assert.IsNotNull(reservations);
            Assert.AreEqual(reservations.Count, 2);
        }

        [TestMethod]
        public void SearchReservations_WithStartDate_ReturnsMatchingReservations()
        {
            // Arrange
            var startDate = DateTime.Now;
            _context.Reservations.Add(new Reservation
            {
                StartDate = startDate,
                EndDate = startDate.AddDays(10),
            });
            _context.Reservations.Add(new Reservation
            {
                StartDate = startDate,
                EndDate = startDate.AddDays(7),
            });
            _context.Reservations.Add(new Reservation
            {
                StartDate = startDate.AddDays(-2),
                EndDate = startDate.AddDays(7),
            });
            _context.SaveChanges();

            // Act
            var result = _reservationService.SearchReservations(startDate: startDate);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.All(r => r.StartDate == startDate));
            Assert.AreEqual(result.Count, 2);
        }

        [TestMethod]
        public void SearchReservations_WithEndDate_ReturnsMatchingReservations()
        {
            // Arrange
            var endDate = DateTime.Now;
            _context.Reservations.Add(new Reservation
            {
                StartDate = endDate.AddDays(-2),
                EndDate = endDate,
            });
            _context.Reservations.Add(new Reservation
            {
                StartDate = endDate.AddDays(-4),
                EndDate = endDate.AddDays(-1),
            });
            _context.SaveChanges();

            // Act
            var result = _reservationService.SearchReservations(endDate: endDate);

            // Assert
            Assert.IsTrue(result.All(r => r.EndDate == endDate));
            Assert.AreEqual(result.Count, 1);
        }

        [TestMethod]
        public void SearchReservations_WithIsPaid_ReturnsMatchingReservations()
        {
            // Arrange
            var isPaid = true;
            _context.Reservations.Add(new Reservation
            {
                StartDate = DateTime.Now.AddDays(-2),
                EndDate = DateTime.Now,
                IsPaid = isPaid
            });
            _context.Reservations.Add(new Reservation
            {
                StartDate = DateTime.Now.AddDays(-4),
                EndDate = DateTime.Now.AddDays(-1),
                IsPaid = !isPaid
            });
            _context.SaveChanges();

            // Act
            var result = _reservationService.SearchReservations(isPaid: isPaid);

            // Assert
            Assert.IsTrue(result.All(r => r.IsPaid == isPaid));
            Assert.AreEqual(result.Count, 1);
        }

        //[TestMethod] // Currencly not working due to missconfiguration of the database
        //public void SearchReservations_WithRooms_ReturnsMatchingReservations()
        //{
        //    // Arrange
        //    List<Room> rooms =
        //       [
        //           new Room
        //           {
        //               Name = "Test Room 1",
        //               PricePerNight = 30.50M
        //           },
        //           new Room
        //           {
        //               Name = "Test Room 2",
        //               PricePerNight = 130.99M
        //           }
        //       ];
        //    _context.Rooms.AddRange(rooms);
        //    _context.SaveChanges();

        //    _context.Reservations.Add(new Reservation
        //    {
        //        StartDate = DateTime.Now.AddDays(-2),
        //        EndDate = DateTime.Now,
        //        Rooms = new List<Room> { _context.Rooms.First(r => r.Name == "Test Room 1") }
        //    });
        //    _context.SaveChanges();

        //    _context.Reservations.Add(new Reservation
        //    {
        //        StartDate = DateTime.Now.AddDays(1),
        //        EndDate = DateTime.Now.AddDays(4),
        //        Rooms = new List<Room> { _context.Rooms.First(r => r.Name == "Test Room 1"), _context.Rooms.First(r => r.Name == "Test Room 2") }
        //    });
        //    _context.SaveChanges();

        //    _context.Reservations.Add(new Reservation
        //    {
        //        StartDate = DateTime.Now.AddDays(4),
        //        EndDate = DateTime.Now.AddDays(10),
        //        Rooms = new List<Room> { _context.Rooms.First(r => r.Name == "Test Room 2") }
        //    });
        //    _context.SaveChanges();

        //    var reservationsInDb = _context.Reservations.ToList();

        //    Assert.AreEqual(3, reservationsInDb[0].Rooms.Count);

        //    List<Room> roomsToFind = [rooms[0]];

        //    // Act
        //    var result = _reservationService.SearchReservations(rooms: roomsToFind);

        //    // Assert
        //    Assert.IsNotNull(result);
        //    Assert.IsTrue(result.All(r => r.Rooms.Any(room => roomsToFind.Contains(room))));
        //    Assert.AreEqual(result.Count, 2);
        //    Assert.AreEqual(result[0].Rooms[0].Name, rooms[0].Name);

        //}
    }
}
