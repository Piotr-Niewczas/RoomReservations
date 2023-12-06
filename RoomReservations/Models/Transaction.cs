using System.ComponentModel.DataAnnotations;

namespace RoomReservations.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        [DataType(DataType.Currency)]
        public decimal Amount { get; set; }
        public DateTime EntryDate { get; set; }
        public DateTime? AccountingDate { get; set; }
    }
}
