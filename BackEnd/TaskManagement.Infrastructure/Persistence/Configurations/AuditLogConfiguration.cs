using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManagement.Domain.Entities;

namespace TaskManagement.Infrastructure.Persistence.Configurations;

// Mirrors the AuditLogs table added to TaskManagement_Database_Schema.sql.
/// <summary>EF Core mapping for <see cref="AuditLog"/>.</summary>
public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    /// <summary>Configures columns, constraints, and the FK to Users.</summary>
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.ToTable("AuditLogs");

        builder.HasKey(a => a.AuditLogId);

        // Same enum-as-string approach as TaskConfiguration - keep the CHECK
        // constraint in the .sql script in sync with AuditAction's member names.
        builder.Property(a => a.Action)
               .HasConversion<string>()
               .HasMaxLength(30)
               .IsRequired();

        builder.Property(a => a.EntityName).HasMaxLength(100);
        builder.Property(a => a.Details).HasMaxLength(500);
        builder.Property(a => a.CreatedDate).HasDefaultValueSql("SYSUTCDATETIME()");

        builder.HasIndex(a => a.UserId);
        builder.HasIndex(a => a.CreatedDate);

        // One-directional - User doesn't need an AuditLogs navigation collection.
        builder.HasOne(a => a.User)
               .WithMany()
               .HasForeignKey(a => a.UserId)
               .OnDelete(DeleteBehavior.SetNull);
    }
}
