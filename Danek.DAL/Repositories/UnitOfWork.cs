using Danek.DAL.Models.Users;
using Danek.DAL.Repositories.Contracts;

namespace Danek.DAL.Repositories
{
    public class UnitOfWork <TEntity, TKey>  : IUnitOfWork <TEntity, TKey>
        where TEntity : class
    {
        private readonly AppDbContext _appDbContext;
        public IGenericRepository<User, Guid> _userRepository;

        private IGenericRepository<TEntity, TKey> _repository;

        public UnitOfWork(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public IGenericRepository<User, Guid> Users => _userRepository ??= new GenericRepository<User, Guid>(_appDbContext);

        public IGenericRepository<TEntity, TKey> Repository => _repository ??= new GenericRepository<TEntity, TKey>(_appDbContext);

        public async Task SaveChangesAsync()
        {
            await _appDbContext.SaveChangesAsync();
        }

        public void Dispose()
        {
            _appDbContext.Dispose();
        }
    }
}
