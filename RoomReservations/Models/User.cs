using System.ComponentModel.DataAnnotations;

namespace RoomReservations.Models
{
    public class User
    {
        public int Id { get; set; }
        [Required(AllowEmptyStrings = false)]
        public string Name { get; set; } = String.Empty;
        [Required(AllowEmptyStrings = false)]
        public string Surname { get; set; } = String.Empty;
        public List<Reservation> Reservations { get; set; } = [];
    }
}
