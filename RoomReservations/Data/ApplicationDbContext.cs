using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RoomReservations.Models;

namespace RoomReservations.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<Reservation> Reservations { get; set; } = null!;
    public DbSet<Room> Rooms { get; set; } = null!;
    public DbSet<Transaction> Transactions { get; set; } = null!;

    public DbSet<RoomReservation> RoomReservations { get; set; } = null!;
    public DbSet<ReservationTransaction> ReservationTransactions { get; set; } = null!;

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

        // Configure User-Reservation relationship
        modelBuilder.Entity<Reservation>()
            .HasOne(r => r.User)
            .WithMany(u => u.Reservations)
            .HasForeignKey(r => r.UserId);
    }
}