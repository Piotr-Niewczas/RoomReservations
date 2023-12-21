using Microsoft.EntityFrameworkCore;
using RoomReservations.Models;

namespace RoomReservations.Data
{
    public class ReservationQuery
    {
        private readonly ApplicationDbContext _context;
        private IQueryable<Reservation> _query;
        public ReservationQuery(ApplicationDbContext context)
        {
            _context = context;
            _query = _context.Reservations;
        }
        public async Task<List<Reservation>> ExecuteAsync()
        {
            return await _query.ToListAsync();
        }

        public ReservationQuery WithRooms()
        {
            _query = _query.Include(r => r.RoomReservations).ThenInclude(rr => rr.Room);
            return this;
        }

        public ReservationQuery WithTransactions()
        {
            _query = _query.Include(r => r.ReservationTransactions).ThenInclude(rt => rt.Transaction);
            return this;
        }

        public ReservationQuery WhereStartDate(DateTime startDate)
        {
            _query = _query.Where(reservation => reservation.StartDate == startDate);
            return this;
        }

        public ReservationQuery WhereEndDate(DateTime endDate)
        {
            _query = _query.Where(reservation => reservation.EndDate == endDate);
            return this;
        }

        public ReservationQuery WhereIsPaid(bool isPaid)
        {
            _query = _query.Where(reservation => reservation.IsPaid == isPaid);
            return this;
        }

        public ReservationQuery WhereAnyOfRooms(List<Room> rooms)
        {
            _query = _query.Where(reservation => reservation.RoomReservations.Any(rr => rooms.Contains(rr.Room)));
            return this;
        }

        public ReservationQuery WhereDatesBetween(DateTime startDate, DateTime endDate)
        {
            _query = _query.Where(reservation => !(startDate >= reservation.EndDate || endDate <= reservation.StartDate));
            return this;
        }
    }

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
