using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Enums;

namespace TaskManagement.Infrastructure.Persistence.Configurations;

/// <summary>EF Core mapping for <see cref="TaskItem"/>.</summary>
public class TaskConfiguration : IEntityTypeConfiguration<TaskItem>
{
    /// <summary>Configures columns, constraints, indexes, and FKs for the Tasks table.</summary>
    public void Configure(EntityTypeBuilder<TaskItem> builder)
    {
        // Entity is named TaskItem (see TaskItem.cs for why); table stays Tasks.
        builder.ToTable("Tasks");

        builder.HasKey(t => t.TaskId);

        builder.Property(t => t.Title).HasMaxLength(200).IsRequired();
        builder.Property(t => t.Description).HasColumnType("nvarchar(max)");

        // HasConversion<string>() stores the enum's member NAME, which is why
        // TaskPriority/TaskItemStatus's member names were chosen to match the
        // SQL CHECK constraints (N'Low'/N'Medium'/N'High', N'ToDo'/N'InProgress'/
        // N'Completed'/N'Cancelled') exactly. If you rename an enum member,
        // update the CHECK constraint in the .sql script to match.
        builder.Property(t => t.Priority)
               .HasConversion<string>()
               .HasMaxLength(20)
               .IsRequired();

        builder.Property(t => t.Status)
               .HasConversion<string>()
               .HasMaxLength(20)
               .HasDefaultValue(TaskItemStatus.ToDo)
               .IsRequired();

        builder.Property(t => t.DueDate).IsRequired();
        builder.Property(t => t.CreatedDate).HasDefaultValueSql("SYSUTCDATETIME()");
        builder.Property(t => t.UpdatedDate).HasDefaultValueSql("SYSUTCDATETIME()");
        builder.Property(t => t.IsDeleted).HasDefaultValue(false);

        builder.HasIndex(t => t.AssignedToUserId);
        builder.HasIndex(t => t.Status);
        builder.HasIndex(t => t.Priority);
        builder.HasIndex(t => t.DueDate);

        // Two independent FKs to Users - this is exactly the AssignedTo /
        // CreatedBy pair from the ERD. Each needs its own HasForeignKey +
        // WithMany so EF Core doesn't try to guess which navigation goes
        // with which column.
        builder.HasOne(t => t.AssignedToUser)
               .WithMany(u => u.AssignedTasks)
               .HasForeignKey(t => t.AssignedToUserId)
               .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(t => t.CreatedByUser)
               .WithMany(u => u.CreatedTasks)
               .HasForeignKey(t => t.CreatedByUserId)
               .OnDelete(DeleteBehavior.Restrict)
               .IsRequired();
    }
}
