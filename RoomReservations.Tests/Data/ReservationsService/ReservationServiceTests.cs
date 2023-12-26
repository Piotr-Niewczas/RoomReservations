using RoomReservations.Data;
using RoomReservations.Models;
using RoomReservations.Tests.Models;

namespace RoomReservations.Tests.Data.ReservationsService
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
            var date = DateTime.Now;
            reservations.Add(new Reservation
            {
                StartDate = date.AddDays(1),
                EndDate = date.AddDays(3),
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
                StartDate = date.AddDays(2),
                EndDate = date.AddDays(5),
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
            var date = DateTime.Now;
            Reservation reservation = new()
            {
                StartDate = date.AddDays(1),
                EndDate = date.AddDays(2),
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
            var date = DateTime.Now;
            List<Room> oneRoom = [rooms[0]];
            Reservation reservation = new()
            {
                StartDate = date.AddDays(1),
                EndDate = date.AddDays(10),
            };
            bool result0 = await _reservationService.AddReservationAsync(reservation, oneRoom);
            Assert.IsTrue(result0);
            await _context.SaveChangesAsync();

            Reservation overlapingReservation = new()
            {
                StartDate = date.AddDays(5),
                EndDate = date.AddDays(15),
            };

            Task<bool> result = _reservationService.AddReservationAsync(overlapingReservation, oneRoom);
            Assert.IsTrue(result.IsCompletedSuccessfully);
            Assert.IsFalse(result.Result);
            // Check that overlaping reservation wasn't added to the database
            List<Reservation> resList = await _reservationService.CreateReservationQuery().ExecuteAsync();
            Assert.AreEqual(resList.Count, 1);
            Assert.AreEqual(resList[0].StartDate, reservation.StartDate);
        }

        [TestMethod]
        public async Task UpdateReservationAsync_ReservationExists_UpdatesAndReturnsTrue()
        {
            var date = DateTime.Now;
            var reservation = new Reservation
            {
                Id = 1,
                StartDate = date,
                EndDate = date.AddDays(1),
                RoomReservations =
                [
                    new RoomReservation
                    {
                        Room = rooms[0]
                    },
                    new RoomReservation
                    {
                        Room = rooms[2]
                    }
                ]
            };
            _context.Reservations.Add(reservation);
            await _context.SaveChangesAsync();
            var updatedReservation = new Reservation
            {
                Id = reservation.Id,
                StartDate = date.AddDays(1),
                EndDate = date.AddDays(2),
                RoomReservations =
                [
                    new RoomReservation
                    {
                        Room = rooms[1]
                    }
                ]
            };

            var result = await _reservationService.UpdateReservationAsync(updatedReservation);

            Assert.IsTrue(result);
            var resFromDb = await _context.Reservations.FindAsync(reservation.Id);
            Assert.IsNotNull(resFromDb);
            Assert.AreEqual(resFromDb.StartDate, updatedReservation.StartDate);
            Assert.AreEqual(resFromDb.EndDate, updatedReservation.EndDate);
            Assert.AreEqual(resFromDb.RoomReservations.Count, updatedReservation.RoomReservations.Count);
            Assert.AreEqual(resFromDb.RoomReservations[0].Room, updatedReservation.RoomReservations[0].Room);
        }

        [TestMethod]
        public async Task UpdateReservationAsync_ReservationDoesNotExist_ReturnsFalse()
        {
            var date = DateTime.Now;
            var updatedReservation = new Reservation
            {
                Id = 1,
                StartDate = date.AddDays(1),
                EndDate = date.AddDays(2),
                RoomReservations =
                [
                    new RoomReservation
                    {
                        Room = rooms[1]
                    }
                ]
            };

            var result = await _reservationService.UpdateReservationAsync(updatedReservation);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task DeleteReservationAsync_ReservationExists_DeletesAndReturnsTrue()
        {
            var date = DateTime.Now;
            var reservation = new Reservation
            {
                Id = 1,
                StartDate = date,
                EndDate = date.AddDays(1),
                RoomReservations =
                [
                    new RoomReservation
                    {
                        Room = rooms[0]
                    }
                ]
            };
            _context.Reservations.Add(reservation);
            await _context.SaveChangesAsync();

            var result = await _reservationService.DeleteReservationAsync(reservation.Id);

            Assert.IsTrue(result);
            var resFromDb = await _context.Reservations.FindAsync(reservation.Id);
            Assert.IsNull(resFromDb);
        }

        [TestMethod]
        public async Task DeleteReservationAsync_ReservationDoesNotExist_ReturnsFalse()
        {
            var result = await _reservationService.DeleteReservationAsync(1);

            Assert.IsFalse(result);
        }

        [TestMethod()]
        public async Task ReservationsForAnyOfRoomsInDateRange_TwoMatching_ReturnsReservations()
        {
            var date = DateTime.Now;
            List<Reservation> reservations = [
                new Reservation
                {
                    StartDate = date,
                    EndDate = date.AddDays(2),
                    RoomReservations =
                    [
                        new RoomReservation
                        {
                            Room = rooms[0]
                        }
                    ]
                },
                new Reservation
                {
                    StartDate = date,
                    EndDate = date.AddDays(4),
                    RoomReservations =
                    [
                        new RoomReservation
                        {
                            Room = rooms[1]
                        }
                    ]
                },
                new Reservation
                {
                    StartDate = date.AddDays(1),
                    EndDate = date.AddDays(3),
                    RoomReservations =
                    [
                        new RoomReservation
                        {
                            Room = rooms[0]
                        },
                        new RoomReservation
                        {
                            Room = rooms[2]
                        }
                    ]
                }
            ];
            _context.Reservations.AddRange(reservations);
            await _context.SaveChangesAsync();

            var result = await _reservationService.ReservationsForAnyOfRoomsInDateRange([rooms[0]], date.AddDays(1), date.AddDays(3));

            Assert.AreEqual(result.Count, 2);
            Assert.IsTrue(result.Contains(reservations[0]));
            Assert.IsTrue(result.Contains(reservations[2]));
        }

        [TestMethod()]
        public async Task ReservationsForAnyOfRoomsInDateRange_NoMatching_ReturnsEmpty()
        {
            var date = DateTime.Now;
            List<Reservation> reservations = [
                new Reservation
                {
                    StartDate = date,
                    EndDate = date.AddDays(2),
                    RoomReservations =
                    [
                        new RoomReservation
                        {
                            Room = rooms[0]
                        }
                    ]
                },
                new Reservation
                {
                    StartDate = date.AddDays(5),
                    EndDate = date.AddDays(6),
                    RoomReservations =
                    [
                        new RoomReservation
                        {
                            Room = rooms[0]
                        }
                    ]
                }];
            _context.Reservations.AddRange(reservations);
            await _context.SaveChangesAsync();

            var result = await _reservationService.ReservationsForAnyOfRoomsInDateRange([rooms[0]], date.AddDays(3), date.AddDays(4));

            Assert.AreEqual(result.Count, 0);
        }

        [TestMethod()]
        public async Task AreAnyRoomsReservedInDateRange_OneMatching_ReturnsTrue()
        {
            var date = DateTime.Now;
            var reservation = new Reservation
            {
                StartDate = date,
                EndDate = date.AddDays(2),
                RoomReservations =
                [
                    new RoomReservation
                    {
                        Room = rooms[0]
                    }
                ]
            };
            _context.Reservations.Add(reservation);
            await _context.SaveChangesAsync();

            var result = await _reservationService.AreAnyRoomsReservedInDateRange([rooms[0]], date.AddDays(2), date.AddDays(3));

            Assert.IsTrue(result);
        }

        [TestMethod()]
        public async Task AreAnyRoomsReservedInDateRange_OneMatchingButIgnored_ReturnsFalse()
        {
            var date = DateTime.Now;
            var reservation = new Reservation
            {
                StartDate = date,
                EndDate = date.AddDays(2),
                RoomReservations =
                [
                    new RoomReservation
                    {
                        Room = rooms[0]
                    }
                ]
            };
            _context.Reservations.Add(reservation);
            await _context.SaveChangesAsync();

            var result = await _reservationService.AreAnyRoomsReservedInDateRange([rooms[0]], date, date.AddDays(3), reservation.Id);

            Assert.IsFalse(result);
        }

        [TestMethod()]
        public async Task AreAnyRoomsReservedInDateRange_NoneMatching_ReturnsFalse()
        {
            var date = DateTime.Now;
            var reservation = new Reservation
            {
                StartDate = date,
                EndDate = date.AddDays(2),
                RoomReservations =
                [
                    new RoomReservation
                    {
                        Room = rooms[0]
                    }
                ]
            };
            _context.Reservations.Add(reservation);
            await _context.SaveChangesAsync();

            var result = await _reservationService.AreAnyRoomsReservedInDateRange([rooms[1]], date.AddDays(2), date.AddDays(3));

            Assert.IsFalse(result);
        }
    }
}