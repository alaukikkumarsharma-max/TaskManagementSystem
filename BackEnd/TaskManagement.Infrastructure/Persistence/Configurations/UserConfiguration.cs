using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManagement.Domain.Entities;

namespace TaskManagement.Infrastructure.Persistence.Configurations;

/// <summary>EF Core mapping for <see cref="User"/>.</summary>
public class UserConfiguration : IEntityTypeConfiguration<User>
{
    /// <summary>Configures columns, constraints, and the FK to Roles.</summary>
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(u => u.UserId);

        builder.Property(u => u.FirstName).HasMaxLength(100).IsRequired();
        builder.Property(u => u.LastName).HasMaxLength(100).IsRequired();
        builder.Property(u => u.Email).HasMaxLength(256).IsRequired();
        builder.Property(u => u.PasswordHash).HasMaxLength(256).IsRequired();
        builder.Property(u => u.IsActive).HasDefaultValue(true);
        builder.Property(u => u.CreatedDate).HasDefaultValueSql("SYSUTCDATETIME()");
        builder.Property(u => u.UpdatedDate).HasDefaultValueSql("SYSUTCDATETIME()");

        builder.HasIndex(u => u.Email).IsUnique();
        builder.HasIndex(u => u.RoleId);

        builder.HasOne(u => u.Role)
               .WithMany(r => r.Users)
               .HasForeignKey(u => u.RoleId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
