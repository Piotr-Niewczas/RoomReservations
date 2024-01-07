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
    Task<Transaction> CreateAdjustmentTransactionAsync(Reservation reservation, decimal howMuchShouldBeLeft);
    decimal GetTotalPaidForReservation(Reservation reservation, bool excludeUnpaid);
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
        UpdateReservationPaidStatus(reservation);
        await context.SaveChangesAsync();
        return reservationTransaction.Transaction.Id;
    }

    public async Task PayTransactionAsync(int transactionId)
    {
        var reservationTransaction = context.ReservationTransactions
            .Include(rt => rt.Transaction)
            .Include(rt => rt.Reservation)
            .First(rt => rt.Transaction.Id == transactionId);
        reservationTransaction.Transaction.AccountingDate = DateTime.Now;
        UpdateReservationPaidStatus(reservationTransaction.Reservation);
        await context.SaveChangesAsync();
    }

    /// <summary>
    ///     Checks how much user has paid for the reservation and creates an adjustment transaction if necessary. Removes all
    ///     unpaid transactions.
    /// </summary>
    /// <param name="reservation">Reservation to get transactions from.</param>
    /// <param name="howMuchShouldBeLeft">What should be total amount paid by user. E.g. 0 for full refund.</param>
    /// <returns>Transaction to cover difference</returns>
    public async Task<Transaction> CreateAdjustmentTransactionAsync(Reservation reservation,
        decimal howMuchShouldBeLeft)
    {
        RemoveUnpaidTransactions(reservation);
        var balance = GetTotalPaidForReservation(reservation, false);

        var diffTransaction = new Transaction
        {
            Amount = howMuchShouldBeLeft - balance,
            EntryDate = DateTime.Now
        };
        if (howMuchShouldBeLeft - balance < 0) // If it's a refund, set accounting date to now
            diffTransaction.AccountingDate = DateTime.Now;
        reservation.ReservationTransactions.Add(new ReservationTransaction
        {
            Reservation = reservation,
            Transaction = diffTransaction
        });
        UpdateReservationPaidStatus(reservation);
        await context.SaveChangesAsync();

        return diffTransaction;
    }

    public decimal GetTotalPaidForReservation(Reservation reservation, bool excludeUnpaid)
    {
        var transactions = reservation.ReservationTransactions
            .Select(rt => rt.Transaction);

        if (excludeUnpaid) transactions = transactions.Where(t => t.AccountingDate != null);

        var balance = transactions.ToList().Sum(t => t.Amount);
        return balance;
    }

    public IQueryable<Transaction> CreateTransactionQuery()
    {
        return new QueryFactory(context).Create<Transaction>();
    }

    private void RemoveUnpaidTransactions(Reservation reservation)
    {
        var unpaidTransactions = reservation.ReservationTransactions
            .Where(rt => rt.Transaction.AccountingDate == null)
            .Select(rt => rt.Transaction)
            .ToList();
        foreach (var unpaidTransaction in unpaidTransactions)
            context.Transactions.Remove(unpaidTransaction);
        context.SaveChanges();
    }

    /// <summary>
    ///     When all positive transactions are paid, set reservation as paid. Does not save changes.
    /// </summary>
    /// <param name="reservation"></param>
    private static void UpdateReservationPaidStatus(Reservation reservation)
    {
        reservation.IsPaid = reservation.ReservationTransactions.Where(rt => rt.Transaction.Amount > 0)
            .All(rt => rt.Transaction.AccountingDate != null);
    }
}