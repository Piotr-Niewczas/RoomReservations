using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace RoomReservations.Models;

// Add profile data for application users by adding properties to the ApplicationUser class
public class ApplicationUser : IdentityUser
{
    public List<Reservation> Reservations { get; set; } = [];

    [Required] public string FirstName { get; set; } = "";

    [Required] public string LastName { get; set; } = "";
}