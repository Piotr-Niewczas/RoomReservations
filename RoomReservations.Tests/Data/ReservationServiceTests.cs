using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoomReservations.Data;
using RoomReservations.Models;
using RoomReservations.Tests.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoomReservations.Data.Tests
{
    [TestClass()]
    public class ReservationServiceTests
    {
        private ApplicationDbContext _context = null!;
        private IReservationService _reservationService = null!;

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

        [TestMethod()]
        public async Task GetReservationsAsyncTest()
        {
            reservations.Add(new Reservation
            {
                StartDate = _date.AddDays(1),
                EndDate = _date.AddDays(2),
                Rooms =
                [
                    rooms[0]
                ],
                Transactions = []
            });
            reservations.Add(new Reservation
            {
                StartDate = _date.AddDays(3),
                EndDate = _date.AddDays(5),
                Rooms =
                [
                    rooms[1]
                ],
                Transactions = []
            });

            _context.Reservations.AddRange(reservations);
            List<Reservation> result = await _reservationService.GetReservationsAsync();

            Assert.IsNotNull(result);
            Assert.AreEqual(result.Count, reservations.Count);
        }

        [TestMethod()]
        public void GetReservationsBetweenAsyncTest()
        {

        }

        [TestMethod()]
        public void AddReservationAsyncTest()
        {

        }

        [TestMethod()]
        public void DeleteAllReservationsTest()
        {

        }

        [TestMethod()]
        public void AreAnyRoomsReservedInDateRangeTest()
        {

        }

        [TestMethod()]
        public void IsRoomReservedInDateRangeTest()
        {

        }

        [TestCleanup]
        public void Cleanup()
        {
            _context.Dispose();
        }
    }
}