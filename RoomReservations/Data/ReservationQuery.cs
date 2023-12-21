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
}
