using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoomReservations.Models
{
    public class Room
    {
        public int Id { get; set; }
        [Required(AllowEmptyStrings = false)]
        public string Name { get; set; } = String.Empty;
        public string Description { get; set; } = String.Empty;
        public int Capacity { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal PricePerNight { get; set; }
        public string Location { get; set; } = String.Empty;
        public string ImageUrl { get; set; } = "img/rooms/notfound.jpg";

        public List<RoomReservation> RoomReservations { get; set; } = [];
    }
}
