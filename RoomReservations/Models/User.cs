using System.ComponentModel.DataAnnotations;

namespace RoomReservations.Models
{
    public class User
    {
        public int Id { get; set; }
        [Required]
        public string? Name { get; set; }
        [Required]
        public string? Surname { get; set; }
        public string? Email { get; set; }
        public ICollection<Reservation>? Reservations { get; set; }
        public UserRoles Role { get; set; } = UserRoles.Client;
    }
}
