using RoomReservations.Data;

namespace RoomReservations.Models;

public class User : ApplicationUser
{
    public List<Reservation> Reservations { get; set; } = [];
}