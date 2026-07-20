using TaskManagement.Domain.Entities;

namespace TaskManagement.Domain.Interfaces;

/// <summary>Role repository - no extra queries beyond the generic CRUD.</summary>
public interface IRoleRepository : IRepository<Role>
{
}
