namespace Danek.BLL.Extesions;

public static class TaskExtension
{
    public static async Task<IEnumerable<T>> WhenAll<T>(this IEnumerable<Task<T>> tasks)
    {
        return await Task.WhenAll(tasks);
    }
}
