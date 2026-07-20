using TaskManagement.Domain.Entities;

namespace TaskManagement.Domain.Interfaces;

/// <summary>User-specific queries on top of the generic repository.</summary>
public interface IUserRepository : IRepository<User>
{
    /// <summary>Fetches an active user by email, with Role loaded.</summary>
    Task<User?> GetByEmailAsync(string email);

    /// <summary>Checks whether an active user exists with this id.</summary>
    Task<bool> ExistsAsync(int userId);
}
