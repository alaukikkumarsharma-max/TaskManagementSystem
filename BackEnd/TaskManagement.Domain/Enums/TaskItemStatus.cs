namespace TaskManagement.Domain.Enums;

// Named TaskItemStatus rather than TaskStatus: System.Threading.Tasks.TaskStatus
// already exists, and every async file in this solution has
// "using System.Threading.Tasks;" in scope. Keeping this name avoids an
// ambiguous-reference compile error the moment both namespaces are imported
// in the same file (which happens constantly - any async method returning
// this enum needs both).
/// <summary>Workflow state of a <see cref="Entities.TaskItem"/>.</summary>
public enum TaskItemStatus
{
    ToDo,
    InProgress,
    Completed,
    Cancelled
}
