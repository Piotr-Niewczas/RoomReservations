using Microsoft.EntityFrameworkCore;
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
    public class RoomServiceTests
    {
        private ApplicationDbContext _context = null!;
        private List<Room> _rooms = [];
        [TestInitialize]
        public void Initialize()
        {

            _context = TestsContextOptions.TestingContext;
            _rooms.Add(new Room
            {
                Name = "Test Room",
                Description = "Test Description",
                Capacity = 2,
                PricePerNight = 30.50M,
                Location = "Test Location",
                ImageUrl = "Test/Image/Url.jpg"
            });
            _rooms.Add(new Room
            {
                Name = "Test Room 2",
                Description = "Test Description 2",
                Capacity = 4,
                PricePerNight = 130.99M,
                Location = "Test Location 2",
                ImageUrl = "Test/Image/Url2.jpg"
            });
            _context.Rooms.AddRange(_rooms);
            _context.SaveChanges();
        }

        [TestMethod()]
        public void GetRoomsAsyncTest()
        {
            var roomService = new RoomService(_context);
            var rooms = roomService.GetRoomsAsync().Result;
            Assert.IsNotNull(rooms);

            Assert.AreEqual(2, _rooms.Count);

            Assert.AreEqual(_rooms[0].Name, rooms[0].Name);
            Assert.AreEqual(_rooms[0].Description, rooms[0].Description);
            Assert.AreEqual(_rooms[0].Capacity, rooms[0].Capacity);
            Assert.AreEqual(_rooms[0].PricePerNight, rooms[0].PricePerNight);
            Assert.AreEqual(_rooms[0].Location, rooms[0].Location);
            Assert.AreEqual(_rooms[0].ImageUrl, rooms[0].ImageUrl);

            Assert.AreEqual(_rooms[1].Name, rooms[1].Name);
            Assert.AreEqual(_rooms[1].Description, rooms[1].Description);
            Assert.AreEqual(_rooms[1].Capacity, rooms[1].Capacity);
            Assert.AreEqual(_rooms[1].PricePerNight, rooms[1].PricePerNight);
            Assert.AreEqual(_rooms[1].Location, rooms[1].Location);
            Assert.AreEqual(_rooms[1].ImageUrl, rooms[1].ImageUrl);

        }

        [TestCleanup]
        public void Cleanup()
        {
            _context.Dispose();
        }
    }
}