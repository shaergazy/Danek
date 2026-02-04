namespace Danek.BLL.Extesions;

public static class IQueryableExtension
{
    public static IQueryable<T> Page<T>(this IQueryable<T> query, int number, int size)
    {
        return query
            .Skip((number - 1) * size)
            .Take(size);
    }
}
