using Microsoft.EntityFrameworkCore;
using RoomReservations.Models;

namespace RoomReservations.Data
{
    public interface IReservationService
    {
        Task<bool> AddReservationAsync(Reservation reservation, List<Room> rooms);
        ReservationQuery CreateReservationQuery();
    }

    public class ReservationService : IReservationService
    {
        private readonly ApplicationDbContext _context;

        public ReservationService(ApplicationDbContext context)
        {
            _context = context;
        }

        public ReservationQuery CreateReservationQuery()
        {
            return new ReservationQuery(_context);
        }

        /// <summary>
        /// Adds Reservation to the database, if it is valid
        /// </summary>
        /// <param name="reservation"></param>
        /// <returns>True if added</returns>
        public async Task<bool> AddReservationAsync(Reservation reservation, List<Room> rooms)
        {
            if (reservation == null)
            {
                return false;
            }

            if (rooms == null || rooms.Count == 0)
            {
                return false;
            }

            if (await AreAnyRoomsReservedInDateRange(rooms, reservation.StartDate, reservation.EndDate))
            {
                return false;
            }

            foreach (var room in rooms)
            {
                reservation.RoomReservations.Add(new RoomReservation
                {
                    Room = room,
                    Reservation = reservation
                });
            }

            _context.Reservations.Add(reservation);
            await _context.SaveChangesAsync();
            return true;

        }

        public async Task<bool> AreAnyRoomsReservedInDateRange(List<Room> rooms, DateTime startdate, DateTime dateTime)
        {
            var reservations = await _context.Reservations
                .Where(r => !(startdate >= r.EndDate || dateTime <= r.StartDate))
                .Where(r => r.RoomReservations.Any(roomInRes => rooms.Contains(roomInRes.Room)))
                .ToListAsync();
            return reservations.Count != 0;
        }
    }
}
