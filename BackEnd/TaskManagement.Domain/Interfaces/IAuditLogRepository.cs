using TaskManagement.Domain.Entities;

namespace TaskManagement.Domain.Interfaces;

/// <summary>Audit log queries on top of the generic repository.</summary>
public interface IAuditLogRepository : IRepository<AuditLog>
{
    /// <summary>Fetches the most recent audit entries, newest first, with the acting user loaded.</summary>
    Task<IReadOnlyList<AuditLog>> GetRecentAsync(int take);
}
