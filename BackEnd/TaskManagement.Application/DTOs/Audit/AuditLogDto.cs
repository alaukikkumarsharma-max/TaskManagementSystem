namespace TaskManagement.Application.DTOs.Audit;

/// <summary>A single audit trail entry as shown on the Audit Logs page.</summary>
public class AuditLogDto
{
    // Primary key.
    public int AuditLogId { get; set; }
    // Who performed the action; "System" when the user is unknown.
    public string UserName { get; set; } = string.Empty;
    // What happened (Login, TaskCreated, TaskUpdated, TaskDeleted).
    public string Action { get; set; } = string.Empty;
    // Affected entity type, if any.
    public string? EntityName { get; set; }
    // Affected entity id, if any.
    public int? EntityId { get; set; }
    // Free-text detail (e.g. the task title at the time).
    public string? Details { get; set; }
    // When the event was recorded (UTC).
    public DateTime CreatedDate { get; set; }
}
