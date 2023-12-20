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
    public class AvailableRoomServiceTests
    {
        private ApplicationDbContext _context = null!;
        private AvailableRoomService _availableRoomService = null!;

        [TestInitialize]
        public void Initialize()
        {
            _context = TestsContextOptions.TestingContext;
            _availableRoomService = new AvailableRoomService(_context);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _context.Dispose();
        }

        [TestMethod()]
        public async Task GetAvailableRoomsAsync_OneBookedRoomAndOneOutOfRange_ReturnsEmpty()
        {
            // Arrange
            var room = new Room
            {
                Name = "Test Room",
                Description = "Test Description",
                Capacity = 10,
                PricePerNight = 100
            };

            _context.Add(room);
            var reservation = new Reservation
            {
                StartDate = DateTime.Now.AddDays(-1),
                EndDate = DateTime.Now.AddDays(1),
                RoomReservations =
                [
                    new RoomReservation
                    {
                        Room = room
                    }
                ]
            };
            _context.Add(reservation);
            _context.SaveChanges();

            // Act
            var result = await _availableRoomService.GetAvailableRoomsAsync(DateTime.Now, DateTime.Now.AddDays(1));

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(result.Count, 0);
        }
        [TestMethod()]
        public async Task GetAvailableRoomsAsync_OneBookedRoomAndOneAvailable_ReturnsOne()
        {
            // Arrange
            var room = new Room
            {
                Name = "Test Room",
                Description = "Test Description",
                Capacity = 10,
                PricePerNight = 100
            };

            _context.Add(room);
            var reservation = new Reservation
            {
                StartDate = DateTime.Now.AddDays(-1),
                EndDate = DateTime.Now.AddDays(1),
                RoomReservations =
                [
                    new RoomReservation
                    {
                        Room = room
                    }
                ]
            };
            _context.Add(reservation);
            _context.SaveChanges();

            // Act
            var result = await _availableRoomService.GetAvailableRoomsAsync(DateTime.Now.AddDays(2), DateTime.Now.AddDays(3));

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(result.Count, 1);
        }
        [TestMethod()]
        public async Task GetAvailableRoomsAsync_TwoRooms_OneAvailable()
        {
            // Arrange
            List<Room> rooms = [
                new Room
                {
                    Name = "Test Room",
                    Description = "Test Description",
                    Capacity = 10,
                    PricePerNight = 100
                },
                new Room
                {
                    Name = "Test Room 2",
                    Description = "Test Description",
                    Capacity = 15,
                    PricePerNight = 150
                }
            ];
            _context.AddRange(rooms);
            var reservation = new Reservation
            {
                StartDate = DateTime.Now.AddDays(-1),
                EndDate = DateTime.Now.AddDays(10),
                RoomReservations =
                [
                    new RoomReservation
                    {
                        Room = rooms[0]
                    }
                ]
            };
            _context.Add(reservation);
            _context.SaveChanges();

            // Act
            var result = await _availableRoomService.GetAvailableRoomsAsync(DateTime.Now.AddDays(2), DateTime.Now.AddDays(3));

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(result.Count, 1);
            Assert.AreEqual(result[0].Name, rooms[1].Name);
        }

    }
}