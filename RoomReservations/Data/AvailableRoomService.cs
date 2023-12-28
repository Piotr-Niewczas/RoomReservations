using Microsoft.EntityFrameworkCore;
using RoomReservations.Models;

namespace RoomReservations.Data;

public interface IAvailableRoomService
{
    Task<List<Room>> GetAvailableRoomsAsync(DateTime startDate, DateTime endDate);
}

public class AvailableRoomService(ApplicationDbContext context) : IAvailableRoomService
{
    private readonly IReservationService _reservationService = new ReservationService(context);
    private readonly IRoomService _roomService = new RoomService(context);


    public async Task<List<Room>> GetAvailableRoomsAsync(DateTime startDate, DateTime endDate)
    {
        var rooms = await _roomService.GetRoomsAsync();
        var reservations = await _reservationService.CreateReservationQuery()
            .WhereDatesBetween(startDate, endDate)
            .WithRooms()
            .ToListAsync();


        return (from room in rooms
            let isAvailable =
                reservations.All(reservation => reservation.RoomReservations.All(rr => rr.RoomId != room.Id))
            where isAvailable
            select room).ToList();
    }
}