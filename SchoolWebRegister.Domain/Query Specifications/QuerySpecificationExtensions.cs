using Microsoft.EntityFrameworkCore;

namespace SchoolWebRegister.Domain.Specifications
{
    public static class QuerySpecificationExtensions
    {
        public static IQueryable<T> Specify<T>(this IQueryable<T> query, ISpecification<T> specification) 
            where T : class
        {
            // Modify the IQueryable using the specification's criteria expression
            if (specification.Criteria != null)
            {
                query = query.Where(specification.Criteria);
            }

            // Includes all expression-based includes
            query = specification.Includes.Aggregate(query,
                                    (current, include) => current.Include(include));

            // Include any string-based include statements
            query = specification.IncludeStrings.Aggregate(query,
                                    (current, include) => current.Include(include));

            // Apply ordering if expressions are set
            if (specification.OrderBy != null)
            {
                query = query.OrderBy(specification.OrderBy);
            }
            else if (specification.OrderByDescending != null)
            {
                query = query.OrderByDescending(specification.OrderByDescending);
            }

            // Apply paging if enabled
            if (specification.isPagingEnabled)
            {
                query = query.Skip(specification.Skip).Take(specification.Take);
            }
            return query;
        }
    }
}
