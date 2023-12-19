using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RoomReservations.Models;

namespace RoomReservations.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
    {
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Transaction> Transactions { get; set; }

        public DbSet<RoomReservation> RoomReservations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

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
        }
    }
}
