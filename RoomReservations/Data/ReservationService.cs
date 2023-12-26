using Microsoft.EntityFrameworkCore;
using RoomReservations.Models;

namespace RoomReservations.Data
{
    public interface IReservationService
    {
        Task<bool> AddReservationAsync(Reservation reservation, List<Room> rooms);
        ReservationQuery CreateReservationQuery();
        Task<bool> UpdateReservationAsync(Reservation updatedReservation);
        Task<bool> DeleteReservationAsync(int id);
        Task<bool> AreAnyRoomsReservedInDateRange(List<Room> rooms, DateTime startdate, DateTime dateTime, int? reservationIdToIgnore = null);
        Task<List<Reservation>> ReservationsForAnyOfRoomsInDateRange(List<Room> rooms, DateTime startdate, DateTime dateTime);
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

        public async Task<bool> AreAnyRoomsReservedInDateRange(List<Room> rooms, DateTime startDate, DateTime endDate, int? reservationIdToIgnore = null)
        {
            var reservations = await ReservationsForAnyOfRoomsInDateRange(rooms, startDate, endDate);

            if (reservationIdToIgnore != null)
            {
                reservations = reservations.Where(r => r.Id != reservationIdToIgnore).ToList();
            }

            return reservations.Any();
        }

        public async Task<List<Reservation>> ReservationsForAnyOfRoomsInDateRange(List<Room> rooms, DateTime startDate, DateTime endDate)
        {
            var reservations = await _context.Reservations
                .Where(r => !(startDate > r.EndDate || endDate < r.StartDate))
                .Where(r => r.RoomReservations.Any(roomInRes => rooms.Contains(roomInRes.Room)))
                .Include(r => r.RoomReservations)
                .ThenInclude(rr => rr.Room)
                .ToListAsync();
            return reservations;
        }

        /// <summary>
        /// Updates reservation in the database, if it is valid. Cannot update reservation transactions.
        /// </summary>
        /// <param name="updatedReservation"></param>
        /// <returns>True if succeded.</returns>
        /// <exception cref="Exception"></exception>
        public async Task<bool> UpdateReservationAsync(Reservation updatedReservation)
        {
            if (updatedReservation == null)
            {
                return false;
            }

            var reservation = await _context.Reservations.FindAsync(updatedReservation.Id);

            if (reservation == null)
            {
                return false;
            }

            if (await AreAnyRoomsReservedInDateRange(
                        updatedReservation.RoomReservations.Select(rr => rr.Room).ToList(),
                        updatedReservation.StartDate,
                        updatedReservation.EndDate,
                        reservation.Id
                    )
                )
            {
                return false;
            }

            reservation.StartDate = updatedReservation.StartDate;
            reservation.EndDate = updatedReservation.EndDate;
            reservation.RoomReservations = updatedReservation.RoomReservations;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ReservationExists(updatedReservation.Id))
                {
                    return false;
                }
                else
                {
                    throw;
                }
            }

            return true;
        }

        private bool ReservationExists(int id)
        {
            return _context.Reservations.Any(e => e.Id == id);
        }

        public async Task<bool> DeleteReservationAsync(int id)
        {
            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation == null)
            {
                return false;
            }

            _context.Reservations.Remove(reservation);
            await _context.SaveChangesAsync();

            return true;
        }

    }
}
