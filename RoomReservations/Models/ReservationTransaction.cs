namespace RoomReservations.Models
{
	public class ReservationTransaction
	{
		public int ReservationId { get; set; }
		public Reservation Reservation { get; set; } = null!;

		public int TransactionId { get; set; }
		public Transaction Transaction { get; set; } = null!;
	}
}
