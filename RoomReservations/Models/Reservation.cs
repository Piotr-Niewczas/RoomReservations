namespace RoomReservations.Models
{
    public class Reservation
    {
        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsPaid { get; set; } = false;
        public List<Room> Rooms { get; set; } = [];
        public List<Transaction> Transactions { get; set; } = [];
    }
}