using Microsoft.EntityFrameworkCore;
using RoomReservations.Data;
using System.Xml.Linq;

namespace RoomReservations.Tests.Models
{
    public class TestsContextOptions
    {
        static DbContextOptions<ApplicationDbContext> Options => new DbContextOptionsBuilder<ApplicationDbContext>()
             .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
             .Options;
        public static ApplicationDbContext TestingContext => new(Options);
    }

}
