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
            var connection = new SqliteConnection("Filename=:memory:");
            connection.Open();
            return new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlite(connection)
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