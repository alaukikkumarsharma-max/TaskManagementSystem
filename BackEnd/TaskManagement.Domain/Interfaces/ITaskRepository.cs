using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Enums;
using TaskManagement.Domain.Models;

namespace TaskManagement.Domain.Interfaces;

/// <summary>Task-specific queries on top of the generic repository.</summary>
public interface ITaskRepository : IRepository<TaskItem>
{
    /// <summary>Returns one page of non-deleted tasks plus the total matching count.</summary>
    Task<(IReadOnlyList<TaskItem> Items, int TotalCount)> SearchAsync(
        string? search,
        TaskItemStatus? status,
        TaskPriority? priority,
        string? sortBy,
        bool sortDescending,
        int page,
        int pageSize);

    /// <summary>Aggregates task counts by status in SQL, without loading any rows.</summary>
    Task<TaskStatistics> GetStatisticsAsync();
}
