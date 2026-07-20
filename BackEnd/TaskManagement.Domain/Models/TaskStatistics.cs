namespace TaskManagement.Domain.Models;

/// <summary>Aggregated task counts, computed in SQL so the dashboard never fetches rows.</summary>
public record TaskStatistics(
    int Total,
    int ToDo,
    int InProgress,
    int Completed,
    int Cancelled,
    int Overdue);
