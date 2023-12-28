using Microsoft.EntityFrameworkCore;
using RoomReservations.Data;

namespace RoomReservations.Tests.Models.Transaction;

[TestClass]
public class Add_Transaction_to_db_Test
{
    private ApplicationDbContext _context = null!;

    [TestInitialize]
    public void Initialize()
    {
        _context = TestsContextOptions.TestingContext;
    }

    [TestMethod]
    public async Task Add_transaction_to_db()
    {
        var transaction = new RoomReservations.Models.Transaction
        {
            Id = 999,
            Amount = 123.45M,
            EntryDate = DateTime.Now,
            AccountingDate = DateTime.Now.AddDays(3)
        };

        _context.Transactions.Add(transaction);
        await _context.SaveChangesAsync();

        var transactionFromDb = await _context.Transactions.FirstOrDefaultAsync(t => t.Id == transaction.Id);

        Assert.IsNotNull(transactionFromDb);

        Assert.AreEqual(transaction.Amount, transactionFromDb.Amount);
        Assert.AreEqual(transaction.EntryDate, transactionFromDb.EntryDate);
        Assert.AreEqual(transaction.AccountingDate, transactionFromDb.AccountingDate);
    }

    [TestCleanup]
    public void Cleanup()
    {
        _context.Dispose();
    }
}