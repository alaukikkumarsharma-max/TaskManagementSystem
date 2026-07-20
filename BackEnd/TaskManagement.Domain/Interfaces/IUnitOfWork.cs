namespace TaskManagement.Domain.Interfaces;

/// <summary>Groups all repositories behind one DbContext/transaction.</summary>
public interface IUnitOfWork
{
    // Task repository.
    ITaskRepository Tasks { get; }
    // User repository.
    IUserRepository Users { get; }
    // Role repository.
    IRoleRepository Roles { get; }
    // Audit log repository.
    IAuditLogRepository AuditLogs { get; }

    /// <summary>Commits all pending changes across every repository.</summary>
    Task<int> SaveChangesAsync();
}
