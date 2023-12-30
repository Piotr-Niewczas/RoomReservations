using Microsoft.EntityFrameworkCore;
using RoomReservations.Models;

namespace RoomReservations.Data;

public static class ReservationQuery
{
    public static IQueryable<Reservation> WithRooms(this IQueryable<Reservation> query)
    {
        return query.Include(r => r.RoomReservations).ThenInclude(rr => rr.Room);
    }

    public static IQueryable<Reservation> WithTransactions(this IQueryable<Reservation> query)
    {
        return query.Include(r => r.ReservationTransactions).ThenInclude(rt => rt.Transaction);
    }

    public static IQueryable<Reservation> WithUsers(this IQueryable<Reservation> query)
    {
        return query.Include(r => r.User);
    }

    public static IQueryable<Reservation> WhereStartDate(this IQueryable<Reservation> query, DateTime startDate)
    {
        return query.Where(reservation => reservation.StartDate == startDate);
    }

    public static IQueryable<Reservation> WhereEndDate(this IQueryable<Reservation> query, DateTime endDate)
    {
        return query.Where(reservation => reservation.EndDate == endDate);
    }

    public static IQueryable<Reservation> WhereIsPaid(this IQueryable<Reservation> query, bool isPaid)
    {
        return query.Where(reservation => reservation.IsPaid == isPaid);
    }

    public static IQueryable<Reservation> WhereAnyOfRooms(this IQueryable<Reservation> query, List<Room> rooms)
    {
        return query.Where(reservation => reservation.RoomReservations.Any(rr => rooms.Contains(rr.Room)));
    }

    public static IQueryable<Reservation> WhereDatesBetween(this IQueryable<Reservation> query, DateTime startDate,
        DateTime endDate)
    {
        return query.Where(reservation => !(startDate >= reservation.EndDate || endDate <= reservation.StartDate));
    }

    public static IQueryable<Reservation> WhereId(this IQueryable<Reservation> query, int id)
    {
        return query.Where(reservation => reservation.Id == id);
    }

    public static IQueryable<Reservation> WhereUserId(this IQueryable<Reservation> query, string id)
    {
        return query.Where(reservation => reservation.UserId == id);
    }
}