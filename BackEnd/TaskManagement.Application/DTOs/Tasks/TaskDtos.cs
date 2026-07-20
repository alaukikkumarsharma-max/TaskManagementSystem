using System.ComponentModel.DataAnnotations;
using TaskManagement.Application.Validation;
using TaskManagement.Domain.Enums;

namespace TaskManagement.Application.DTOs.Tasks;

/// <summary>A task as returned to API clients.</summary>
public class TaskDto
{
    // Primary key.
    public int TaskId { get; set; }
    // Short title.
    public string Title { get; set; } = string.Empty;
    // Optional longer description.
    public string? Description { get; set; }
    // Low/Medium/High urgency.
    public TaskPriority Priority { get; set; }
    // Current workflow state.
    public TaskItemStatus Status { get; set; }
    // Date the task is due.
    public DateTime DueDate { get; set; }
    // Id of the assigned user, if any.
    public int? AssignedToUserId { get; set; }
    // Display name of the assigned user, if any.
    public string? AssignedToName { get; set; }
    // Id of the user who created the task.
    public int CreatedByUserId { get; set; }
    // Display name of the creator.
    public string CreatedByName { get; set; } = string.Empty;
    // Timestamp the task was created.
    public DateTime CreatedDate { get; set; }
    // Timestamp of the last change.
    public DateTime UpdatedDate { get; set; }
}

/// <summary>Payload for creating a new task.</summary>
public class CreateTaskDto
{
    [Required(ErrorMessage = "Title is required.")]
    [MaxLength(200)]
    // Short title.
    public string Title { get; set; } = string.Empty;

    // Optional longer description.
    public string? Description { get; set; }

    [Required(ErrorMessage = "Priority is required.")]
    // Low/Medium/High urgency.
    public TaskPriority Priority { get; set; }

    [Required(ErrorMessage = "Due date is required.")]
    [FutureDate(ErrorMessage = "Due date cannot be in the past.")]
    // Date the task is due; must not be in the past.
    public DateTime DueDate { get; set; }

    // Id of the user to assign, if any.
    public int? AssignedToUserId { get; set; }
}

/// <summary>Payload for updating an existing task.</summary>
public class UpdateTaskDto
{
    [Required(ErrorMessage = "Title is required.")]
    [MaxLength(200)]
    // Short title.
    public string Title { get; set; } = string.Empty;

    // Optional longer description.
    public string? Description { get; set; }

    [Required(ErrorMessage = "Priority is required.")]
    // Low/Medium/High urgency.
    public TaskPriority Priority { get; set; }

    [Required(ErrorMessage = "Status is required.")]
    // Current workflow state.
    public TaskItemStatus Status { get; set; }

    [Required(ErrorMessage = "Due date is required.")]
    [FutureDate(ErrorMessage = "Due date cannot be in the past.")]
    // Date the task is due; must not be in the past.
    public DateTime DueDate { get; set; }

    // Id of the user to assign, if any.
    public int? AssignedToUserId { get; set; }
}
