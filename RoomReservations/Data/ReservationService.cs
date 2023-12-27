using Microsoft.EntityFrameworkCore;
using RoomReservations.Models;

namespace RoomReservations.Data;

public interface IReservationService
{
    Task<bool> AddReservationAsync(Reservation reservation, List<Room> rooms);
    Task<bool> UpdateReservationAsync(Reservation updatedReservation);
    Task<bool> DeleteReservationAsync(int id);
}

public class ReservationService(ApplicationDbContext context) : IReservationService
{
    /// <summary>
    ///     Adds Reservation to the database, if it is valid
    /// </summary>
    /// <param name="reservation"></param>
    /// <param name="rooms"></param>
    /// <returns>True if added</returns>
    public async Task<bool> AddReservationAsync(Reservation reservation, List<Room> rooms)
    {
        if (rooms.Count == 0) return false;

        if (ReservationsForAnyOfRoomsInDateRange(rooms,
                reservation.StartDate,
                reservation.EndDate)
            .Any())
            return false;

        foreach (var room in rooms)
            reservation.RoomReservations.Add(new RoomReservation
            {
                Room = room,
                Reservation = reservation
            });

        context.Reservations.Add(reservation);
        await context.SaveChangesAsync();
        return true;
    }

    /// <summary>
    ///     Updates reservation in the database, if it is valid. Cannot update reservation transactions.
    /// </summary>
    /// <param name="updatedReservation"></param>
    /// <returns>True if succeeded.</returns>
    /// <exception cref="Exception"></exception>
    public async Task<bool> UpdateReservationAsync(Reservation updatedReservation)
    {
        var reservation = await context.Reservations.FindAsync(updatedReservation.Id);
        if (reservation == null) return false;

        var collidingReservations = await ReservationsForAnyOfRoomsInDateRange(updatedReservation.RoomReservations
                    .Select(rr => rr.Room)
                    .ToList(),
                updatedReservation.StartDate,
                updatedReservation.EndDate)
            .Where(r => r.Id != reservation.Id).ToListAsync();

        if (collidingReservations.Count != 0) return false;

        reservation.StartDate = updatedReservation.StartDate;
        reservation.EndDate = updatedReservation.EndDate;
        reservation.RoomReservations = updatedReservation.RoomReservations;

        try
        {
            await context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!ReservationExists(updatedReservation.Id))
                return false;
            throw;
        }

        return true;
    }

    public async Task<bool> DeleteReservationAsync(int id)
    {
        var reservation = await context.Reservations.FindAsync(id);
        if (reservation == null) return false;

        context.Reservations.Remove(reservation);
        await context.SaveChangesAsync();

        return true;
    }

    private IQueryable<Reservation> ReservationsForAnyOfRoomsInDateRange(List<Room> rooms, DateTime startDate,
        DateTime endDate)
    {
        var reservations = context.Reservations
            .Where(r => !(startDate > r.EndDate || endDate < r.StartDate))
            .Where(r => r.RoomReservations.Any(roomInRes => rooms.Contains(roomInRes.Room)))
            .Include(r => r.RoomReservations)
            .ThenInclude(rr => rr.Room);

        return reservations;
    }

    private bool ReservationExists(int id)
    {
        return context.Reservations.Any(e => e.Id == id);
    }
}