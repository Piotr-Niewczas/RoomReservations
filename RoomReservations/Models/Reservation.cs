namespace RoomReservations.Models
{
    public class Reservation
    {
        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsPaid { get; set; } = false;
        public ICollection<Room>? Rooms { get; set; }
        public ICollection<Transaction>? transactions { get; set; }
    }
}