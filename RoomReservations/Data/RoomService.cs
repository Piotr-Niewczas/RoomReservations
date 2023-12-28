using Microsoft.EntityFrameworkCore;
using RoomReservations.Models;

namespace RoomReservations.Data;

public interface IRoomService
{
    Task<List<Room>> GetRoomsAsync();
}

public class RoomService(ApplicationDbContext context) : IRoomService
{
    public async Task<List<Room>> GetRoomsAsync()
    {
        return await context.Rooms.ToListAsync();
    }
}