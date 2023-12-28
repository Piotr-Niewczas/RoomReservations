using Microsoft.EntityFrameworkCore;
using RoomReservations.Models;

namespace RoomReservations.Data;

public interface IReservationService
{
    Task<bool> AddReservationAsync(Reservation reservation, List<Room> rooms);
    ReservationQuery CreateReservationQuery();
    Task<bool> UpdateReservationAsync(Reservation updatedReservation);
    Task<bool> DeleteReservationAsync(int id);

    Task<bool> AreAnyRoomsReservedInDateRange(List<Room> rooms, DateTime startdate, DateTime dateTime,
        int? reservationIdToIgnore = null);

    Task<List<Reservation>> ReservationsForAnyOfRoomsInDateRange(List<Room> rooms, DateTime startdate,
        DateTime dateTime);
}

public class ReservationService(ApplicationDbContext context) : IReservationService
{
    public ReservationQuery CreateReservationQuery()
    {
        return new ReservationQuery(context);
    }

    /// <summary>
    ///     Adds Reservation to the database, if it is valid
    /// </summary>
    /// <param name="reservation"></param>
    /// <returns>True if added</returns>
    public async Task<bool> AddReservationAsync(Reservation reservation, List<Room> rooms)
    {
        if (rooms.Count == 0) return false;

        if (await AreAnyRoomsReservedInDateRange(rooms, reservation.StartDate, reservation.EndDate)) return false;

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

    public async Task<bool> AreAnyRoomsReservedInDateRange(List<Room> rooms, DateTime startDate, DateTime endDate,
        int? reservationIdToIgnore = null)
    {
        var reservations = await ReservationsForAnyOfRoomsInDateRange(rooms, startDate, endDate);

        if (reservationIdToIgnore != null)
            reservations = reservations.Where(r => r.Id != reservationIdToIgnore).ToList();

        return reservations.Count != 0;
    }

    public async Task<List<Reservation>> ReservationsForAnyOfRoomsInDateRange(List<Room> rooms, DateTime startDate,
        DateTime endDate)
    {
        var reservations = await context.Reservations
            .Where(r => !(startDate > r.EndDate || endDate < r.StartDate))
            .Where(r => r.RoomReservations.Any(roomInRes => rooms.Contains(roomInRes.Room)))
            .Include(r => r.RoomReservations)
            .ThenInclude(rr => rr.Room)
            .ToListAsync();
        return reservations;
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

        if (await AreAnyRoomsReservedInDateRange(
                updatedReservation.RoomReservations.Select(rr => rr.Room).ToList(),
                updatedReservation.StartDate,
                updatedReservation.EndDate,
                reservation.Id
            )
           )
            return false;

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

    private bool ReservationExists(int id)
    {
        return context.Reservations.Any(e => e.Id == id);
    }
}