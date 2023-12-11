using Microsoft.EntityFrameworkCore;
using RoomReservations.Models;

namespace RoomReservations.Data
{
	public interface IAvailableRoomService
	{
		Task<List<Room>> GetAvailableRoomsAsync(DateTime startDate, DateTime endDate);
	}

	public class AvailableRoomService : IAvailableRoomService
	{
		private readonly IReservationService _reservationService;
		private readonly IRoomService _roomService;


		public AvailableRoomService(ApplicationDbContext context)
		{
			_reservationService = new ReservationService(context);
			_roomService = new RoomService(context);
		}

		public async Task<List<Room>> GetAvailableRoomsAsync(DateTime startDate, DateTime endDate)
		{
			var rooms = await _roomService.GetRoomsAsync();
			var reservations = await _reservationService.GetReservationsBetweenAsync(startDate, endDate);
			var availableRooms = new List<Room>();
			foreach (var room in rooms)
			{
				var isAvailable = true;
				foreach (var reservation in reservations)
				{
					if (reservation.Rooms.Any(roomInRes => roomInRes.Id == room.Id))
					{
						isAvailable = false;
						break;
					}
				}

				if (isAvailable)
				{
					availableRooms.Add(room);
				}
			}

			return availableRooms;
		}
	}
}
