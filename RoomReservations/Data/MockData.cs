using Microsoft.EntityFrameworkCore;
using RoomReservations.Models;

namespace RoomReservations.Data
{
    public static class MockData
    {
        public static List<Room> MockRooms => new List<Room>
               {
                new() {
                    Id = 1,
                    Name = "Room 1",
                    Description = "Generic room Description.",
                    Capacity = 2,
                    PricePerNight = 100,
                    Location = "1st Floor",
                    ImageUrl = "img/rooms/room1.jpg" },
                new() {
                    Id = 2,
                    Name = "Room 2",
                    Description = "Woman and laptop not included.",
                    Capacity = 3,
                    PricePerNight = 99.99M,
                    Location = "1st Floor",
                    ImageUrl = "img/rooms/room2.jpg" },
                new() {
                    Id = 3,
                    Name = "Room 3",
                    Description = "White and teal room with a (sometimes) working TV.",
                    Capacity = 4,
                    PricePerNight = 234,
                    Location = "1st Floor",
                    ImageUrl = "img/rooms/room3.jpg" },
                new() {
                    Id = 4,
                    Name = "Basement Nr.4",
                    Description = "The budget option.",
                    Capacity = 1,
                    PricePerNight = 50.50M,
                    Location = "Basement",
                    ImageUrl = "img/rooms/room4.jpg" },
                new() {
                    Id = 5,
                    Name = "VIP Room 5",
                    Description = "Very nice room for very rude people.",
                    Capacity = 4,
                    PricePerNight = 1000,
                    Location = "Helipad",
                    ImageUrl = "img/rooms/room5.jpg" },
                new() {
                    Id = 6,
                    Name = "Room 6",
                    Description = "R e d.",
                    Capacity = 2,
                    PricePerNight = 100,
                    Location = "Location 6",
                    ImageUrl = "img/rooms/room6.jpg" },
                new() {
                    Id = 7,
                    Name = "Room 7",
                    Description = "Big mirror with an attached room.",
                    Capacity = 2,
                    PricePerNight = 100,
                    Location = "Middle Floor",
                    ImageUrl = "img/rooms/room7.jpg" },
                new() {
                    Id = 8,
                    Name = "Room 8",
                    Description = "Very spacious room with dedicated AC.",
                    Capacity = 4,
                    PricePerNight = 259.99M,
                    Location = "2nd Floor",
                    ImageUrl = "img/rooms/room8.jpg" },
                new() {
                    Id = 9,
                    Name = "Room 9",
                    Description = "Kinda small with wooden fan.",
                    Capacity = 2,
                    PricePerNight = 99.97M,
                    Location = "N+1 Floor",
                    ImageUrl = "img/rooms/room9.jpg"}
               };
        public static void AddMockDataIfNonePresent(ApplicationDbContext dbContext)
        {
            if (dbContext.Rooms.Any())
            {
                return;
            }
            dbContext.Rooms.AddRange(MockRooms);
            dbContext.SaveChanges();
        }
    }
}
