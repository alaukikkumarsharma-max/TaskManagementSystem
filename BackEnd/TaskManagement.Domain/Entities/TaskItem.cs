using TaskManagement.Domain.Enums;

namespace TaskManagement.Domain.Entities;

// Named TaskItem, not Task: System.Threading.Tasks.Task is used constantly
// for async return types throughout this codebase, so naming the entity
// "Task" would collide the moment a file needs both. Same reasoning as
// TaskItemStatus above. Maps to the "Tasks" table either way.
/// <summary>A unit of work that can be assigned to a user and tracked to completion.</summary>
public class TaskItem
{
    // Primary key.
    public int TaskId { get; set; }
    // Short title shown in lists.
    public string Title { get; set; } = string.Empty;
    // Optional longer description.
    public string? Description { get; set; }
    // Low/Medium/High urgency.
    public TaskPriority Priority { get; set; }
    // Current workflow state.
    public TaskItemStatus Status { get; set; } = TaskItemStatus.ToDo;
    // Date the task is due.
    public DateTime DueDate { get; set; }
    // User this task is assigned to, if any.
    public int? AssignedToUserId { get; set; }
    // User who created this task.
    public int CreatedByUserId { get; set; }
    // Timestamp the task was created.
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    // Timestamp of the last change.
    public DateTime UpdatedDate { get; set; } = DateTime.UtcNow;
    // True if soft-deleted (excluded from normal reads).
    public bool IsDeleted { get; set; }

    // Navigation to the assignee.
    public User? AssignedToUser { get; set; }
    // Navigation to the creator.
    public User CreatedByUser { get; set; } = null!;

    /// <summary>Stamps <see cref="UpdatedDate"/> with the current UTC time.</summary>
    public void MarkUpdated() => UpdatedDate = DateTime.UtcNow;

    // The one piece of real business logic this entity has: it protects its
    // own invariant regardless of who is calling it. The API's
    // [FutureDate] attribute on the incoming DTO is what actually stops a
    // bad request before it gets this far (see CreateTaskDto/UpdateTaskDto)
    // - this is the defense-in-depth backstop, not the primary check.
    /// <summary>Throws if <see cref="DueDate"/> is in the past.</summary>
    public void EnsureDueDateNotInPast()
    {
        if (DueDate.Date < DateTime.UtcNow.Date)
        {
            throw new ArgumentException("Due date cannot be in the past.");
        }
    }
}
