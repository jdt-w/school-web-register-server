﻿using System.Linq.Expressions;
using SchoolWebRegister.Domain.Specifications;

namespace SchoolWebRegister.DAL.Repositories
{
    public interface IDbRepository<T>
    {
        Task<T> AddAsync(T entity);
        IQueryable<T> Specify(ISpecification<T> spec);
        Task DeleteAsync(T entity);
        IQueryable<T> Select();
        Task<IEnumerable<T>> SelectAsync();
        Task<IEnumerable<T>> SelectAsync(ISpecification<T> spec);
        Task<IEnumerable<T>> SelectAsync(Expression<Func<T, bool>> predicate);
        Task<T?> GetByIdAsync(string id);
        Task<T> UpdateAsync(T entity);
        Task<int> CountAsync(ISpecification<T> spec);
    }
}
