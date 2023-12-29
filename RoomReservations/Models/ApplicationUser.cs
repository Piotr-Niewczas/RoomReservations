using Microsoft.AspNetCore.Identity;

namespace RoomReservations.Models;

// Add profile data for application users by adding properties to the ApplicationUser class
public class ApplicationUser : IdentityUser
{
    public List<Reservation> Reservations { get; set; } = [];
}