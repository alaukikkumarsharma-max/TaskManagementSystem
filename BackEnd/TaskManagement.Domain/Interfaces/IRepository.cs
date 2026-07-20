namespace TaskManagement.Domain.Interfaces;

/// <summary>Generic CRUD operations shared by every entity repository.</summary>
public interface IRepository<T> where T : class
{
    /// <summary>Fetches a single entity by id, or null if not found.</summary>
    Task<T?> GetByIdAsync(int id);

    /// <summary>Fetches every entity of this type.</summary>
    Task<IReadOnlyList<T>> GetAllAsync();

    /// <summary>Stages a new entity for insertion.</summary>
    Task AddAsync(T entity);

    /// <summary>Stages changes to an existing entity.</summary>
    void Update(T entity);

    /// <summary>Stages an entity for removal.</summary>
    void Remove(T entity);
}
