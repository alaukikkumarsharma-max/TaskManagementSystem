using Microsoft.EntityFrameworkCore;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Interfaces;
using TaskManagement.Infrastructure.Persistence;

namespace TaskManagement.Infrastructure.Repositories;

/// <summary>User-specific queries: email lookup, existence check, active-user listing.</summary>
public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(AppDbContext context) : base(context)
    {
    }

    // Role is included here because AuthService reads user.Role.Name to
    // build the JWT role claim and the login response right after this call.
    /// <summary>Fetches an active user by email, with Role loaded.</summary>
    public async Task<User?> GetByEmailAsync(string email) =>
        await DbSet.Include(u => u.Role)
                    .FirstOrDefaultAsync(u => u.Email == email && u.IsActive);

    // Same reasoning as GetByEmailAsync - UserService reads u.Role.Name for
    // the /api/users list that populates the task-form assignee dropdown.
    /// <summary>Fetches every active user, with Role loaded.</summary>
    public override async Task<IReadOnlyList<User>> GetAllAsync() =>
        await DbSet.Include(u => u.Role)
                    .Where(u => u.IsActive)
                    .ToListAsync();

    /// <summary>Checks whether an active user exists with this id.</summary>
    public async Task<bool> ExistsAsync(int userId) =>
        await DbSet.AnyAsync(u => u.UserId == userId && u.IsActive);
}
