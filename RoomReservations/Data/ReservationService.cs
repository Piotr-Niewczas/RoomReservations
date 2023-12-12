using Microsoft.EntityFrameworkCore;
using RoomReservations.Models;

namespace RoomReservations.Data
{
	public interface IReservationService
	{
		Task<bool> AddReservationAsync(Reservation reservation);
		Task<List<Reservation>> GetReservationsAsync();
		Task<List<Reservation>> GetReservationsBetweenAsync(DateTime startDate, DateTime endDate);
		Task DeleteAllReservations();
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
		public async Task<bool> AddReservationAsync(Reservation reservation)
		{
			if (reservation == null)
			{
				return false;
			}

			if (reservation.Rooms == null || reservation.Rooms.Count == 0)
			{
				return false;
			}

			if (await AreAnyRoomsReservedInDateRange(reservation.Rooms, reservation.StartDate, reservation.EndDate))
			{
				return false;
			}

			_context.Reservations.Add(reservation);
			await _context.SaveChangesAsync();
			return true;

		}
		public async Task DeleteAllReservations()
		{
			_context.Reservations.RemoveRange(_context.Reservations);
			await _context.SaveChangesAsync();
		}

		private async Task<bool> AreAnyRoomsReservedInDateRange(List<Room> rooms, DateTime startdate, DateTime dateTime)
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

		private async Task<bool> IsRoomReservedInDateRange(Room room, DateTime startdate, DateTime dateTime)
		{
			var reservations = await _context.Reservations
				.Where(r => !(startdate >= r.EndDate || dateTime <= r.StartDate))
				.Where(r => r.Rooms.Any(roomInRes => roomInRes.Id == room.Id))
				.ToListAsync();
			return reservations.Any();
		}
	}
}
