using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManagement.Domain.Entities;

namespace TaskManagement.Infrastructure.Persistence.Configurations;

// Mirrors TaskManagement_Database_Schema.sql exactly - that script is the
// source of truth for the schema; this just teaches EF Core to talk to it.
/// <summary>EF Core mapping for <see cref="Role"/>.</summary>
public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    /// <summary>Configures columns and constraints for the Roles table.</summary>
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("Roles");

        builder.HasKey(r => r.RoleId);

        builder.Property(r => r.Name)
               .HasMaxLength(50)
               .IsRequired();

        builder.HasIndex(r => r.Name).IsUnique();
    }
}
