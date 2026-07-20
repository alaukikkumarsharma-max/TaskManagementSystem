using TaskManagement.Application.DTOs.Audit;
using TaskManagement.Domain.Enums;

namespace TaskManagement.Application.Interfaces;

/// <summary>Records and reads audit trail events (logins, task changes).</summary>
public interface IAuditService
{
    /// <summary>Writes a single audit log entry.</summary>
    Task LogAsync(int? userId, AuditAction action, string? entityName = null, int? entityId = null, string? details = null);

    /// <summary>Returns the most recent audit entries for the Audit Logs page.</summary>
    Task<IReadOnlyList<AuditLogDto>> GetRecentAsync(int take = 100);
}
