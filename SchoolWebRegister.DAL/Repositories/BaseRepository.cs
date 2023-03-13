using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using SchoolWebRegister.Domain.Specifications;

namespace SchoolWebRegister.DAL.Repositories
{
    public abstract class BaseRepository<T> : IDbRepository<T> where T : class
    {
        private readonly DbContext _dbContext;

        public BaseRepository(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<T> AddAsync(T entity)
        {
            _dbContext.Set<T>().Add(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }
        public IQueryable<T> Specify(ISpecification<T> spec)
        {
            return _dbContext.Set<T>().AsQueryable().Specify(spec);
        }
        public async Task<int> CountAsync(ISpecification<T> spec)
        {
            return await Specify(spec).CountAsync();
        }
        public async Task DeleteAsync(T entity)
        {
            _dbContext.Set<T>().Remove(entity);
            await _dbContext.SaveChangesAsync();
        }
        public async Task<T?> GetByIdAsync(string id)
        {
            return await _dbContext.Set<T>().FindAsync(id);
        }
        public async Task<IEnumerable<T>> SelectAsync()
        {
            return await _dbContext.Set<T>().ToListAsync();
        }
        public async Task<IEnumerable<T>> SelectAsync(ISpecification<T> spec)
        {
            return await Specify(spec).ToListAsync();
        }
        public async Task<IEnumerable<T>> SelectAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbContext.Set<T>().Where(predicate).ToListAsync();
        }
        public async Task UpdateAsync(T entity)
        {
            _dbContext.Set<T>().Update(entity);
            await _dbContext.SaveChangesAsync();
        }
    }
}
