namespace RoomReservations.Data;

public class QueryFactory(ApplicationDbContext context)
{
    public IQueryable<T> Create<T>() where T : class
    {
        return context.Set<T>().AsQueryable();
    }
}