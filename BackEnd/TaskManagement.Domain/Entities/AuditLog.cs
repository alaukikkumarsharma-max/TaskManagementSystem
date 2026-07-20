using TaskManagement.Domain.Enums;

namespace TaskManagement.Domain.Entities;

/// <summary>A single recorded audit event (login, task created/updated/deleted).</summary>
public class AuditLog
{
    // Primary key.
    public int AuditLogId { get; set; }
    // User who performed the action, if known.
    public int? UserId { get; set; }
    // What happened.
    public AuditAction Action { get; set; }
    // Name of the affected entity type, if any.
    public string? EntityName { get; set; }
    // Id of the affected entity, if any.
    public int? EntityId { get; set; }
    // Free-text detail (e.g. the task title at the time).
    public string? Details { get; set; }
    // Timestamp the event was recorded.
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    // Navigation to the acting user.
    public User? User { get; set; }
}
