using TaskManagement.Application.DTOs.Tasks;
using TaskManagement.Domain.Entities;

namespace TaskManagement.Application.Mappings;

/// <summary>Maps <see cref="TaskItem"/> entities to their DTO representation.</summary>
public static class TaskMappingExtensions
{
    // Assumes AssignedToUser / CreatedByUser navigation properties are
    // already loaded (TaskRepository always includes them) - if they
    // aren't, this silently produces empty names instead of throwing,
    // which is deliberate here but worth knowing if you extend this.
    /// <summary>Converts a task entity into its response DTO.</summary>
    public static TaskDto ToDto(this TaskItem task) => new()
    {
        TaskId = task.TaskId,
        Title = task.Title,
        Description = task.Description,
        Priority = task.Priority,
        Status = task.Status,
        DueDate = task.DueDate,
        AssignedToUserId = task.AssignedToUserId,
        AssignedToName = task.AssignedToUser is null
            ? null
            : $"{task.AssignedToUser.FirstName} {task.AssignedToUser.LastName}",
        CreatedByUserId = task.CreatedByUserId,
        CreatedByName = task.CreatedByUser is null
            ? string.Empty
            : $"{task.CreatedByUser.FirstName} {task.CreatedByUser.LastName}",
        CreatedDate = task.CreatedDate,
        UpdatedDate = task.UpdatedDate
    };
}
