using Microsoft.EntityFrameworkCore;
using RoomReservations.Models;

namespace RoomReservations.Data;

public interface IRoomService
{
    Task<List<Room>> GetRoomsAsync();
}

public class RoomService : IRoomService
{
    private readonly ApplicationDbContext _context;

    public RoomService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Room>> GetRoomsAsync()
    {
        return await _context.Rooms.ToListAsync();
    }
}