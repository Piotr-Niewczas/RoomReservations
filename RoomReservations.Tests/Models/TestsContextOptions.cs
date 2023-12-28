using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using RoomReservations.Data;

namespace RoomReservations.Tests.Models;

public class TestsContextOptions
{
    private static DbContextOptions<ApplicationDbContext> Options
    {
        get
        {
            var _connection = new SqliteConnection("Filename=:memory:");
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