using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RoomReservations.Models;

namespace RoomReservations.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<Reservation> Reservations { get; set; }
    public DbSet<Room> Rooms { get; set; }
    public DbSet<Transaction> Transactions { get; set; }

    public DbSet<RoomReservation> RoomReservations { get; set; }
    public DbSet<ReservationTransaction> ReservationTransactions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Many-to-many relationship between Room and Reservation
        modelBuilder.Entity<RoomReservation>()
            .HasKey(rr => new { rr.RoomId, rr.ReservationId });

        modelBuilder.Entity<RoomReservation>()
            .HasOne(rr => rr.Room)
            .WithMany(r => r.RoomReservations)
            .HasForeignKey(rr => rr.RoomId);

        modelBuilder.Entity<RoomReservation>()
            .HasOne(rr => rr.Reservation)
            .WithMany(re => re.RoomReservations)
            .HasForeignKey(rr => rr.ReservationId);

        // Many-to-many relationship between Reservation and Transaction
        modelBuilder.Entity<ReservationTransaction>()
            .HasKey(rt => new { rt.ReservationId, rt.TransactionId });

        modelBuilder.Entity<ReservationTransaction>()
            .HasOne(rt => rt.Reservation)
            .WithMany(r => r.ReservationTransactions)
            .HasForeignKey(rt => rt.ReservationId);

        modelBuilder.Entity<ReservationTransaction>()
            .HasOne(rt => rt.Transaction)
            .WithMany(t => t.ReservationTransactions)
            .HasForeignKey(rt => rt.TransactionId);
    }
}