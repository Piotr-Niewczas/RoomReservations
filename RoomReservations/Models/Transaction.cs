using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoomReservations.Models
{
	public class Transaction
	{
		public int Id { get; set; }
		[Column(TypeName = "decimal(18,2)")]
		public decimal Amount { get; set; }
		public DateTime EntryDate { get; set; }
		public DateTime? AccountingDate { get; set; }

		public List<ReservationTransaction> ReservationTransactions { get; set; } = [];
	}
}
