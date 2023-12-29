using Microsoft.AspNetCore.Identity;
using RoomReservations.Models;

namespace RoomReservations.Data;

// Add profile data for application users by adding properties to the ApplicationUser class
public class ApplicationUser : IdentityUser
{
    public List<Reservation> Reservations { get; set; } = [];
}