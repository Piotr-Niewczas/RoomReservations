using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RoomReservations.Models;

namespace RoomReservations.Data;

public static class MockData
{
    private static readonly List<UserData> MockUsers =
    [
        new UserData("admin", "Admin123", "admin@admin.com", "Admin"),
        new UserData("receptionist", "Receptionist123", "recepcionist@r.com", "Receptionist"),
        new UserData("employee", "Employee123", "employee@e.com", "Employee"),
        new UserData("client", "Client123", "client@c.com", "Client")
    ];

    private static readonly List<Room> MockRooms =
    [
        new Room
        {
            Id = 1,
            Name = "Room 1",
            Description = "Generic room Description.",
            Capacity = 2,
            PricePerNight = 100,
            Location = "1st Floor",
            ImageUrl = "img/rooms/room1.jpg"
        },

        new Room
        {
            Id = 2,
            Name = "Room 2",
            Description = "Woman and laptop not included.",
            Capacity = 3,
            PricePerNight = 99.99M,
            Location = "1st Floor",
            ImageUrl = "img/rooms/room2.jpg"
        },

        new Room
        {
            Id = 3,
            Name = "Room 3",
            Description = "White and teal room with a (sometimes) working TV.",
            Capacity = 4,
            PricePerNight = 234,
            Location = "1st Floor",
            ImageUrl = "img/rooms/room3.jpg"
        },

        new Room
        {
            Id = 4,
            Name = "Basement Nr.4",
            Description = "The budget option.",
            Capacity = 1,
            PricePerNight = 50.50M,
            Location = "Basement",
            ImageUrl = "img/rooms/room4.jpg"
        },

        new Room
        {
            Id = 5,
            Name = "VIP Room 5",
            Description = "Very nice room for very rude people.",
            Capacity = 4,
            PricePerNight = 1000,
            Location = "Helipad",
            ImageUrl = "img/rooms/room5.jpg"
        },

        new Room
        {
            Id = 6,
            Name = "Room 6",
            Description = "R e d.",
            Capacity = 2,
            PricePerNight = 100,
            Location = "Location 6",
            ImageUrl = "img/rooms/room6.jpg"
        },

        new Room
        {
            Id = 7,
            Name = "Room 7",
            Description = "Big mirror with an attached room.",
            Capacity = 2,
            PricePerNight = 100,
            Location = "Middle Floor",
            ImageUrl = "img/rooms/room7.jpg"
        },

        new Room
        {
            Id = 8,
            Name = "Room 8",
            Description = "Very spacious room with dedicated AC.",
            Capacity = 4,
            PricePerNight = 259.99M,
            Location = "2nd Floor",
            ImageUrl = "img/rooms/room8.jpg"
        },

        new Room
        {
            Id = 9,
            Name = "Room 9",
            Description = "Kinda small with wooden fan.",
            Capacity = 2,
            PricePerNight = 99.97M,
            Location = "N+1 Floor",
            ImageUrl = "img/rooms/room9.jpg"
        }
    ];

    public static void AddMockDataIfNonePresent(ApplicationDbContext dbContext)
    {
        if (dbContext.Rooms.Any()) return;
        dbContext.Rooms.AddRange(MockRooms);
        dbContext.SaveChanges();
    }

    public static async Task AddMockUsersIfNonePresent(IServiceScope scope)
    {
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        if (await userManager.Users.AnyAsync()) return; // If there are any users, don't add mock users

        foreach (var user in MockUsers)
        {
            var applicationUser = new ApplicationUser
            {
                UserName = user.UserName,
                Email = user.Email
            };

            await userManager.CreateAsync(applicationUser, user.Password);
            await userManager.AddToRoleAsync(applicationUser, user.Role);
        }
    }

    private class UserData(string userName, string password, string email, string role)
    {
        public string UserName { get; } = userName;
        public string Password { get; } = password;
        public string Email { get; } = email;
        public string Role { get; } = role;
    }
}