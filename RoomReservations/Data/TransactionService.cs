using Microsoft.EntityFrameworkCore;
using RoomReservations.Models;

namespace RoomReservations.Data;

public interface ITransactionService
{
    /// <summary>
    ///     Add transaction to the database
    /// </summary>
    /// <param name="reservation"> Reservation to add transaction to </param>
    /// <param name="amount"> Amount </param>
    /// <returns> ID of added transaction </returns>
    Task<int> AddTransactionAsync(Reservation reservation, decimal amount);

    Task PayTransactionAsync(int transactionId);
    IQueryable<Transaction> CreateTransactionQuery();
}

public class TransactionService(ApplicationDbContext context) : ITransactionService
{
    public async Task<int> AddTransactionAsync(Reservation reservation, decimal amount)
    {
        var reservationTransaction = new ReservationTransaction
        {
            Reservation = reservation,
            Transaction = new Transaction
            {
                Amount = amount,
                EntryDate = DateTime.Now
            }
        };
        reservation.ReservationTransactions.Add(reservationTransaction);
        await context.SaveChangesAsync();
        return reservationTransaction.Transaction.Id;
    }

    public async Task PayTransactionAsync(int transactionId)
    {
        var reservationTransaction = context.ReservationTransactions
            .Include(rt => rt.Transaction)
            .First(rt => rt.Transaction.Id == transactionId);
        reservationTransaction.Transaction.AccountingDate = DateTime.Now;
        await context.SaveChangesAsync();
    }

    public IQueryable<Transaction> CreateTransactionQuery()
    {
        return new QueryFactory(context).Create<Transaction>();
    }
}