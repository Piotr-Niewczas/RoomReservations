using RoomReservations.Data;
using RoomReservations.Models;
using RoomReservations.Tests.Models;

namespace RoomReservations.Tests.Data.ReservationsServiceTests
{
    [TestClass()]
    public class ReservationServiceTests
    {
        private ApplicationDbContext _context = null!;
        private ReservationService _reservationService = null!;

        readonly List<Room> rooms =
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
        readonly List<Reservation> reservations = [];
        private readonly DateTime _date = new(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc);

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
        public async Task GetReservationsAsync_TwoReservationsInDb_ReturnsTwoReservations()
        {
            reservations.Add(new Reservation
            {
                StartDate = _date.AddDays(1),
                EndDate = _date.AddDays(3),
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
                StartDate = _date.AddDays(2),
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

            List<Reservation> result = await _reservationService.CreateReservationQuery().ExecuteAsync();

            Assert.IsNotNull(result);
            Assert.AreEqual(result.Count, reservations.Count);
        }

        [TestMethod()]
        public async Task GetReservationsAsync_NoReservationsInDb_ReturnsEmptyList()
        {
            List<Reservation> result = await _reservationService.CreateReservationQuery().ExecuteAsync();

            Assert.IsNotNull(result);
            Assert.AreEqual(result.Count, 0);
        }

        [TestMethod()]
        public async Task GetReservationsBetweenAsync_TwoReservations_OneMatching()
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

            List<Reservation> result = await _reservationService.CreateReservationQuery()
                .WhereDatesBetween(_date.AddDays(0), _date.AddDays(2))
                .ExecuteAsync();

            Assert.IsNotNull(result);
            Assert.AreEqual(result.Count, 1);
            Assert.AreEqual(result[0].StartDate, reservations[0].StartDate);
        }

        [TestMethod()]
        public async Task GetReservationsBetweenAsync_TwoReservations_NoneMatching()
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

            List<Reservation> result = await _reservationService.CreateReservationQuery()
                .WhereDatesBetween(_date.AddDays(3), _date.AddDays(4))
                .ExecuteAsync(); ;

            Assert.IsNotNull(result);
            Assert.AreEqual(result.Count, 0);
        }

        [TestMethod()]
        public async Task AddReservationAsync_NullArgument_ReturnsFalse()
        {
            Reservation reservation = null!;
            List<Room> rooms = new();

            Task<bool> result = _reservationService.AddReservationAsync(reservation, rooms);

            Assert.IsTrue(result.IsCompletedSuccessfully);
            Assert.IsFalse(result.Result);
            // Check that no reservations were added to the database
            List<Reservation> resList = await _reservationService.CreateReservationQuery().ExecuteAsync();
            Assert.AreEqual(resList.Count, 0);
        }

        [TestMethod()]
        public async Task AddReservationAsync_RoomsListEmpty_ReturnsFalse()
        {
            Reservation reservation = new()
            {
                StartDate = _date.AddDays(1),
                EndDate = _date.AddDays(2),
            };
            List<Room> rooms = new();

            Task<bool> result = _reservationService.AddReservationAsync(reservation, rooms);

            Assert.IsTrue(result.IsCompletedSuccessfully);
            Assert.IsFalse(result.Result);
            // Check that no reservations were added to the database
            List<Reservation> resList = await _reservationService.CreateReservationQuery().ExecuteAsync();
            Assert.AreEqual(resList.Count, 0);
        }

        [TestMethod()]
        public async Task AddReservationAsync_TwoOverlapingReservations_ReturnsFalse()
        {
            List<Room> oneRoom = [rooms[0]];
            Reservation reservation = new()
            {
                StartDate = _date.AddDays(1),
                EndDate = _date.AddDays(10),
            };
            bool result0 = await _reservationService.AddReservationAsync(reservation, oneRoom);
            Assert.IsTrue(result0);
            await _context.SaveChangesAsync();

            Reservation overlapingReservation = new()
            {
                StartDate = _date.AddDays(5),
                EndDate = _date.AddDays(15),
            };

            Task<bool> result = _reservationService.AddReservationAsync(overlapingReservation, oneRoom);
            Assert.IsTrue(result.IsCompletedSuccessfully);
            Assert.IsFalse(result.Result);
            // Check that overlaping reservation wasn't added to the database
            List<Reservation> resList = await _reservationService.CreateReservationQuery().ExecuteAsync();
            Assert.AreEqual(resList.Count, 1);
            Assert.AreEqual(resList[0].StartDate, reservation.StartDate);
        }

    }
}