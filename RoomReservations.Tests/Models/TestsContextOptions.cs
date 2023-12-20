using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using RoomReservations.Data;
using System.Xml.Linq;

namespace RoomReservations.Tests.Models
{
    public class TestsContextOptions
    {
        static DbContextOptions<ApplicationDbContext> Options
        {
            get
            {
                SqliteConnection _connection = new SqliteConnection("Filename=:memory:");
                _connection.Open();
                return new DbContextOptionsBuilder<ApplicationDbContext>()
                 .UseSqlite(_connection)
                 .Options;
            }
        }

        public static ApplicationDbContext TestingContext
        {
            get
            {
                var context = new ApplicationDbContext(Options);
                context.Database.EnsureCreated();
                return context;
            }
        }
    }

}
