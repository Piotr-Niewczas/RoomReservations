using RoomReservations.Models;
using RoomReservations.Tests.Models;

namespace RoomReservations.Data.Tests;

[TestClass]
public class RoomServiceTests
{
    private readonly List<Room> _rooms = [];
    private ApplicationDbContext _context = null!;
    private RoomService _roomService = null!;

    [TestInitialize]
    public void Initialize()
    {
        _context = TestsContextOptions.TestingContext;
        _roomService = new RoomService(_context);
    }

    [TestMethod]
    public void GetRoomsAsyncTest()
    {
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

        var rooms = _roomService.GetRoomsAsync().Result;

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