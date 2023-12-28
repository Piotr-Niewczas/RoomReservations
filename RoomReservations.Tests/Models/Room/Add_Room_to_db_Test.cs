using Microsoft.EntityFrameworkCore;
using RoomReservations.Data;

namespace RoomReservations.Tests.Models.Room;

[TestClass]
public class AddRoomToDbTest
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