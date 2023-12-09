using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;

namespace RoomReservations.Tests.Models
{
    internal class RRDbContext(DbContextOptions<RRDbContext> options) : DbContext(options)
    {
        public DbSet<RoomReservations.Models.Room> Rooms { get; set; }
        public DbSet<RoomReservations.Models.Reservation> Reservations { get; set; }
        public DbSet<RoomReservations.Models.Transaction> Transactions { get; set; }
        public static DbContextOptions<RRDbContext> DefaultOptions = new DbContextOptionsBuilder<RRDbContext>()
                .UseInMemoryDatabase("RRTests")
                .Options;
    }
}
