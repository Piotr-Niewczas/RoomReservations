namespace RoomReservations.Models
{
    public class RoomReservation
    {
        public int RoomId { get; set; }
        public Room Room { get; set; } = null!;

        public int ReservationId { get; set; }
        public Reservation Reservation { get; set; } = null!;
    }
}
