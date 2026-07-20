using Microsoft.EntityFrameworkCore;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Interfaces;
using TaskManagement.Infrastructure.Persistence;

namespace TaskManagement.Infrastructure.Repositories;

/// <summary>Audit log repository with a newest-first recent query.</summary>
public class AuditLogRepository : Repository<AuditLog>, IAuditLogRepository
{
    public AuditLogRepository(AppDbContext context) : base(context)
    {
    }

    // User is included so the Audit Logs page can show who did what without a second query.
    /// <summary>Fetches the most recent audit entries, newest first, with the acting user loaded.</summary>
    public async Task<IReadOnlyList<AuditLog>> GetRecentAsync(int take) =>
        await DbSet.Include(a => a.User)
                    .OrderByDescending(a => a.CreatedDate)
                    .Take(take)
                    .ToListAsync();
}
