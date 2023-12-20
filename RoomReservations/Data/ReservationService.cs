using Microsoft.EntityFrameworkCore;
using RoomReservations.Models;

namespace RoomReservations.Data
{
    public interface IReservationService
    {
        Task<bool> AddReservationAsync(Reservation reservation, List<Room> rooms);
        Task<List<Reservation>> GetReservationsAsync();
        Task<List<Reservation>> GetReservationsBetweenAsync(DateTime startDate, DateTime endDate);
    }

    public class ReservationService : IReservationService
    {
        private readonly ApplicationDbContext _context;

        public ReservationService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Reservation>> GetReservationsAsync()
        {
            return await _context.Reservations.ToListAsync();
        }

        public async Task<List<Reservation>> GetReservationsBetweenAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.Reservations
                .Where(r => !(startDate >= r.EndDate || endDate <= r.StartDate))
                .ToListAsync();
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
            foreach (var room in rooms)
            {
                if (await IsRoomReservedInDateRange(room, startdate, dateTime))
                {
                    return true;
                }
            }
            return false;
        }

        public async Task<bool> IsRoomReservedInDateRange(Room room, DateTime startdate, DateTime dateTime)
        {
            var reservations = await _context.Reservations
                .Where(r => !(startdate >= r.EndDate || dateTime <= r.StartDate))
                .Where(r => r.RoomReservations.Any(roomInRes => roomInRes.RoomId == room.Id))
                .ToListAsync();
            return reservations.Count != 0;
        }

        public List<Reservation> SearchReservations(DateTime? startDate = null, DateTime? endDate = null, bool? isPaid = null, List<Room>? rooms = null)
        {
            var query = _context.Reservations.AsQueryable();

            if (startDate is not null && startDate.HasValue)
            {
                query = query.Where(reservation => reservation.StartDate == startDate.Value);
            }

            if (endDate is not null && endDate.HasValue)
            {
                query = query.Where(reservation => reservation.EndDate == endDate.Value);
            }

            if (isPaid is not null && isPaid.HasValue)
            {
                query = query.Where(reservation => reservation.IsPaid == isPaid.Value);
            }

            if (rooms is not null && rooms.Count > 0)
            {
                query = query.Where(reservation => reservation.RoomReservations.Any(rr => rooms.Contains(rr.Room)));
            }

            return query.ToList();
        }
    }
}
