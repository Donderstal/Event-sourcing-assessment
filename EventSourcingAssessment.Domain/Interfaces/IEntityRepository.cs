using System.Linq.Expressions;

namespace EventSourcingAssessment.Domain.Interfaces;

public interface IEntityRepository<T>
{
    Task<T?> GetByIdAsync(Guid id, params Expression<Func<T, object>>[] includes);

    Task<List<T>> GetAllAsync(params Expression<Func<T, object>>[] includes);
    
    Task AddAsync(T entity);

    Task UpdateAsync(T entity);
}