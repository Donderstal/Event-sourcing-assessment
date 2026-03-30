using System.Linq.Expressions;
using EventSourcingAssessment.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EventSourcingAssessment.Persistence.Repositories;

public class EntityRepository<T>(InMemoryDbContext dbContext) : IEntityRepository<T>
    where T : class
{
    private readonly DbSet<T> _entities = dbContext.Set<T>();
    
    public async Task<T?> GetByIdAsync(Guid id, params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> query = _entities;

        foreach (var include in includes)
        {
            query = query.Include(include);
        }

        return await query.FirstOrDefaultAsync(e => EF.Property<Guid>(e, "Id") == id);
    }
    
    public async Task<List<T>> GetAllAsync(params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> query = _entities;

        foreach (var include in includes)
        {
            query = query.Include(include);
        }

        return await query.ToListAsync();
    }
    
    public async Task AddAsync(T entity)
    {
        _entities.Add(entity);
        await dbContext.SaveChangesAsync();
    }
    
    public async Task UpdateAsync(T entity)
    {
        _entities.Update(entity);
        await dbContext.SaveChangesAsync();
    }
}