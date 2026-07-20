using Microsoft.EntityFrameworkCore;
using TaskManagement.Domain.Interfaces;
using TaskManagement.Infrastructure.Persistence;

namespace TaskManagement.Infrastructure.Repositories;

/// <summary>Generic EF Core repository shared by every entity type.</summary>
public class Repository<T> : IRepository<T> where T : class
{
    // EF Core context.
    protected readonly AppDbContext Context;
    // Set for this entity type.
    protected readonly DbSet<T> DbSet;

    public Repository(AppDbContext context)
    {
        Context = context;
        DbSet = context.Set<T>();
    }

    // virtual: TaskRepository overrides both to filter out soft-deleted
    // rows and to include the AssignedToUser/CreatedByUser navigations.
    /// <summary>Fetches a single entity by id, or null if not found.</summary>
    public virtual async Task<T?> GetByIdAsync(int id) => await DbSet.FindAsync(id);

    /// <summary>Fetches every entity of this type.</summary>
    public virtual async Task<IReadOnlyList<T>> GetAllAsync() => await DbSet.ToListAsync();

    /// <summary>Stages a new entity for insertion.</summary>
    public async Task AddAsync(T entity) => await DbSet.AddAsync(entity);

    /// <summary>Stages changes to an existing entity.</summary>
    public void Update(T entity) => DbSet.Update(entity);

    /// <summary>Stages an entity for removal.</summary>
    public void Remove(T entity) => DbSet.Remove(entity);
}
