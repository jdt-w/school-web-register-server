using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using SchoolWebRegister.Domain.Specifications;

namespace SchoolWebRegister.DAL.Repositories
{
    public abstract class BaseRepository<T> : IDbRepository<T> where T : class
    {
        private readonly DbContext Context;

        public BaseRepository(DbContext dbContext)
        {
            Context = dbContext;
        }

        public async Task<T> AddAsync(T entity)
        {
            Context.Set<T>().Add(entity);
            await Context.SaveChangesAsync();
            return entity;
        }
        public IQueryable<T> Specify(ISpecification<T> spec)
        {
            return Context.Set<T>().AsQueryable().Specify(spec);
        }
        public async Task<int> CountAsync(ISpecification<T> spec)
        {
            return await Specify(spec).CountAsync();
        }
        public async Task DeleteAsync(T entity)
        {
            Context.Set<T>().Remove(entity);
            await Context.SaveChangesAsync();
        }
        public async Task<T?> GetByIdAsync(string id)
        {
            return await Context.Set<T>().FindAsync(id);
        }
        public IQueryable<T> Select()
        {
            return Context.Set<T>().AsQueryable<T>();
        }
        public async Task<IEnumerable<T>> SelectAsync()
        {
            return await Context.Set<T>().ToListAsync();
        }
        public async Task<IEnumerable<T>> SelectAsync(ISpecification<T> spec)
        {
            return await Specify(spec).ToListAsync();
        }
        public async Task<IEnumerable<T>> SelectAsync(Expression<Func<T, bool>> predicate)
        {
            return await Context.Set<T>().Where(predicate).ToListAsync();
        }
        public async Task<T> UpdateAsync(T entity)
        {
            var result = Context.Set<T>().Update(entity);
            await Context.SaveChangesAsync();
            return result.Entity;
        }
    }
}
