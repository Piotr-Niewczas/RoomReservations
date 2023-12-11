using Microsoft.EntityFrameworkCore;
using RoomReservations.Models;

namespace RoomReservations.Data
{
	public interface IReservationService
	{
		Task<Reservation> AddReservationAsync(Reservation reservation);
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

		public async Task<Reservation> AddReservationAsync(Reservation reservation)
		{
			_context.Reservations.Add(reservation);
			await _context.SaveChangesAsync();
			return reservation;
		}
	}
}
