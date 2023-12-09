using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RoomReservations.Data;

namespace RoomReservations.Tests.Models.Room
{
    [TestClass]
    public class Add_Room_to_db_Test
    {
        private ApplicationDbContext _context = null!;
        [TestInitialize]
        public void Initialize()
        {
            _context = TestsContextOptions.TestingContext;
        }

        [TestMethod]
        public async Task Add_Room_to_db()
        {
            var room = new RoomReservations.Models.Room
            {
                Name = "Test Room",
                Description = "Test Description",
                Capacity = 10,
                PricePerNight = 100.25M,
                Location = "Test Location",
                ImageUrl = "Test/Image/Url.jpg"
            };

            _context.Rooms.Add(room);
            await _context.SaveChangesAsync();

            var roomFromDb = await _context.Rooms.FirstOrDefaultAsync(r => r.Name == "Test Room");

            Assert.IsNotNull(roomFromDb);

            Assert.AreEqual(room.Name, roomFromDb.Name);
            Assert.AreEqual(room.Description, roomFromDb.Description);
            Assert.AreEqual(room.Capacity, roomFromDb.Capacity);
            Assert.AreEqual(room.PricePerNight, roomFromDb.PricePerNight);
            Assert.AreEqual(room.Location, roomFromDb.Location);
            Assert.AreEqual(room.ImageUrl, roomFromDb.ImageUrl);

        }

        [TestCleanup]
        public void Cleanup()
        {
            _context.Dispose();
        }
    }
}
