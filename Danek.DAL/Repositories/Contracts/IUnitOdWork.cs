namespace Danek.DAL.Repositories.Contracts
{
    public interface IUnitOfWork <TEntity, TKey> : IDisposable
        where TEntity : class
    {
        public IGenericRepository<TEntity, TKey> Repository { get; }
        Task SaveChangesAsync();
    }
}
