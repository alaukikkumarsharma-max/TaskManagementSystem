using Microsoft.EntityFrameworkCore;
using TaskManagement.Domain.Entities;

namespace TaskManagement.Infrastructure.Persistence;

/// <summary>EF Core context for the TaskManagement database.</summary>
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    // Roles table.
    public DbSet<Role> Roles => Set<Role>();
    // Users table.
    public DbSet<User> Users => Set<User>();
    // Tasks table.
    public DbSet<TaskItem> Tasks => Set<TaskItem>();
    // AuditLogs table.
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    /// <summary>Applies every IEntityTypeConfiguration in this assembly.</summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
