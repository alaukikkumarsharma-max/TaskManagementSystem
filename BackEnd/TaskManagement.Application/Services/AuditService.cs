using TaskManagement.Application.DTOs.Audit;
using TaskManagement.Application.Interfaces;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Enums;
using TaskManagement.Domain.Interfaces;

namespace TaskManagement.Application.Services;

/// <summary>Records audit trail events (logins, task changes).</summary>
public class AuditService : IAuditService
{
    private readonly IUnitOfWork _unitOfWork;

    public AuditService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    /// <summary>Writes a single audit log entry.</summary>
    public async Task LogAsync(int? userId, AuditAction action, string? entityName = null, int? entityId = null, string? details = null)
    {
        var entry = new AuditLog
        {
            UserId = userId,
            Action = action,
            EntityName = entityName,
            EntityId = entityId,
            Details = details
        };

        await _unitOfWork.AuditLogs.AddAsync(entry);
        await _unitOfWork.SaveChangesAsync();
    }

    /// <summary>Returns the most recent audit entries for the Audit Logs page.</summary>
    public async Task<IReadOnlyList<AuditLogDto>> GetRecentAsync(int take = 100)
    {
        var logs = await _unitOfWork.AuditLogs.GetRecentAsync(take);

        return logs
            .Select(a => new AuditLogDto
            {
                AuditLogId = a.AuditLogId,
                // User is null when the row's UserId FK was cleared (SET NULL on user delete).
                UserName = a.User is null ? "System" : $"{a.User.FirstName} {a.User.LastName}",
                Action = a.Action.ToString(),
                EntityName = a.EntityName,
                EntityId = a.EntityId,
                Details = a.Details,
                CreatedDate = a.CreatedDate
            })
            .ToList();
    }
}
