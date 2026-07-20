namespace TaskManagement.Domain.Enums;

/// <summary>The kind of event an <see cref="Entities.AuditLog"/> row records.</summary>
public enum AuditAction
{
    Login,
    TaskCreated,
    TaskDeleted,
    TaskUpdated
}
