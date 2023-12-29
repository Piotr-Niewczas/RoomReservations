using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RoomReservations.Data;
using RoomReservations.Models;

namespace RoomReservations.Tests.Models;

[TestClass]
public class AddUserReservationsToDb
{
    private ApplicationDbContext _context = null!;
    private UserManager<ApplicationUser> _userManager = null!;

    [TestInitialize]
    public void Initialize()
    {
        _context = TestsContextOptions.TestingContext;

        var userStore = new UserStore<ApplicationUser>(_context);
        _userManager = new UserManager<ApplicationUser>(userStore, null!, new PasswordHasher<ApplicationUser>(),
            Array.Empty<IUserValidator<ApplicationUser>>(), Array.Empty<IPasswordValidator<ApplicationUser>>(),
            new UpperInvariantLookupNormalizer(),
            new IdentityErrorDescriber(), null!, new Logger<UserManager<ApplicationUser>>(new LoggerFactory()));
    }


    [TestMethod]
    public async Task AddUserTest_AddsUserToDb_ReturnsUser()
    {
        // Arrange
        var user = new ApplicationUser { Email = "test@test.com", UserName = "test@test.com" };
        await _userManager.CreateAsync(user);

        // Act
        var result = await _userManager.FindByNameAsync("test@test.com");

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(user.Email, result.Email);
    }

    [TestMethod]
    public async Task AddReservationWithUser_ReservationLinksUser()
    {
        // Arrange
        var user = new ApplicationUser { Email = "test@test.com", UserName = "test@test.com" };
        await _userManager.CreateAsync(user);

        var reservation = new RoomReservations.Models.Reservation
        {
            Id = 999,
            StartDate = DateTime.Now,
            EndDate = DateTime.Now.AddDays(3),
            IsPaid = false,
            UserId = user.Id
        };
        await _context.Reservations.AddAsync(reservation);
        await _context.SaveChangesAsync();

        // Act
        var result = await _context.Reservations.Include(r => r.User).FirstOrDefaultAsync(r => r.Id == reservation.Id);
        Assert.IsNotNull(result);
        var resultUser = result.User;

        // Assert
        Assert.IsNotNull(resultUser);
        Assert.AreEqual(user.Email, resultUser.Email);
    }

    [TestMethod]
    public async Task AddReservationWithUser_UserLinksReservation()
    {
        // Arrange
        var user = new ApplicationUser { Email = "test@test.com", UserName = "test@test.com" };
        await _userManager.CreateAsync(user);

        var reservation = new RoomReservations.Models.Reservation
        {
            Id = 999,
            StartDate = DateTime.Now,
            EndDate = DateTime.Now.AddDays(3),
            IsPaid = false,
            UserId = user.Id
        };
        await _context.Reservations.AddAsync(reservation);
        await _context.SaveChangesAsync();

        // Act
        var result = await _context.Users.Include(u => u.Reservations).FirstOrDefaultAsync(u => u.Id == user.Id);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Reservations.Count);
        Assert.AreEqual(reservation.Id, result.Reservations[0].Id);
        Assert.AreEqual(reservation.StartDate, result.Reservations[0].StartDate);
        Assert.AreEqual(reservation.EndDate, result.Reservations[0].EndDate);
        Assert.AreEqual(reservation.IsPaid, result.Reservations[0].IsPaid);
        Assert.AreEqual(reservation.UserId, result.Reservations[0].UserId);
        Assert.AreEqual(user.Email, result.Email);
    }
}