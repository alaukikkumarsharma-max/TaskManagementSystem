namespace TaskManagement.Application.DTOs.Tasks;

/// <summary>Dashboard counters, aggregated in SQL rather than by fetching every task.</summary>
public class TaskStatsDto
{
    // All non-deleted tasks.
    public int Total { get; set; }
    // Tasks not started yet.
    public int ToDo { get; set; }
    // Tasks currently being worked on.
    public int InProgress { get; set; }
    // Finished tasks.
    public int Completed { get; set; }
    // Abandoned tasks.
    public int Cancelled { get; set; }
    // Unfinished tasks whose due date has passed.
    public int Overdue { get; set; }
}
