using TaskManagement.Domain.Interfaces;
using TaskManagement.Infrastructure.Persistence;

namespace TaskManagement.Infrastructure.Repositories;

/// <summary>Groups all repositories behind one DbContext/transaction.</summary>
public class UnitOfWork : IUnitOfWork, IDisposable
{
    private readonly AppDbContext _context;
    private ITaskRepository? _tasks;
    private IUserRepository? _users;
    private IRoleRepository? _roles;
    private IAuditLogRepository? _auditLogs;

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
    }

    // Lazy-instantiated, but all share this same _context - so a
    // TaskService call that touches Tasks and Users still commits through
    // one SaveChangesAsync() against one connection/transaction.
    // Task repository.
    public ITaskRepository Tasks => _tasks ??= new TaskRepository(_context);
    // User repository.
    public IUserRepository Users => _users ??= new UserRepository(_context);
    // Role repository.
    public IRoleRepository Roles => _roles ??= new RoleRepository(_context);
    // Audit log repository.
    public IAuditLogRepository AuditLogs => _auditLogs ??= new AuditLogRepository(_context);

    /// <summary>Commits all pending changes across every repository.</summary>
    public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();

    /// <summary>Disposes the underlying DbContext.</summary>
    public void Dispose()
    {
        _context.Dispose();
        GC.SuppressFinalize(this);
    }
}
