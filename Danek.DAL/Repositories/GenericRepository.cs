using System.Linq.Expressions;
using Danek.DAL.Repositories.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Danek.DAL.Repositories
{
    public class GenericRepository<TEntity, TKey> : IGenericRepository <TEntity, TKey> 
    where TEntity : class
    {
        protected readonly DbSet<TEntity> _dbSet;
        protected readonly AppDbContext _context;
        public GenericRepository(AppDbContext context)
        {
            _context = context;
            _dbSet = context.Set<TEntity>();
            
        }

        public IQueryable<TEntity> GetAll()
        {
            return _dbSet.AsQueryable();
        }

        public async Task<TEntity> GetByIdAsync(TKey id)
        {
           return await _dbSet.FindAsync(id);
        }
            
        public async Task<TEntity> AddAsync(TEntity model)
        {
            var entity = (await _context.AddAsync(model)).Entity;

            return entity;
        }

        public async Task DeleteByIdAsync(TKey id)
        {
            TEntity entity = await _dbSet.FindAsync(id);
           _context.Remove(entity);
        }

        public async Task UpdateAsync(TEntity model)
        {
            var entity = _dbSet.Update(model).Entity;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public IQueryable<TEntity> Where(Expression<Func<TEntity, bool>> expression = null)
        {
            return _dbSet.Where(expression).AsQueryable();
        }

        public async Task AddRangeAsync(ICollection<TEntity> entities)
        {
            await _dbSet.AddRangeAsync(entities);
        }

        public bool Any(Expression<Func<TEntity, bool>> expression = null)
        {
            return _dbSet.Any(expression);
        }

        public Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<TEntity, TProperty> Include<TProperty>(Expression<Func<TEntity, TProperty>> expression)
        {
            return _dbSet.Include(expression);
        }

        public IQueryable<TEntity> AsNoTracking()
        {
            return _dbSet.AsNoTracking();
        }

        public async Task Remove(TEntity model)
        {
             _dbSet.Remove(model);
        }

        public virtual void AddRange(IEnumerable<TEntity> entities)
        {
            _dbSet.AddRange(entities);
        }

        public async Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> expression = null)
        {
            return await _dbSet.FirstOrDefaultAsync(expression);
        }

        public void RemoveRange(IEnumerable<TEntity> entities)
        {
            _dbSet.RemoveRange(entities);
        }

        public IEnumerable<TEntity> AsEnumerable()
        {
            return _dbSet.AsEnumerable();
        }

        public async Task<TEntity> FindAsync(TKey id)
        {
            return await _dbSet.FindAsync(id);
        }

        public EntityEntry<TEntity> Entry(TEntity entity)
        {
            return _dbSet.Entry(entity);
        }
    }
}
