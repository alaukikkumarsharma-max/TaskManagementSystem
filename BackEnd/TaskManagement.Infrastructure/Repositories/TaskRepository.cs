using Microsoft.EntityFrameworkCore;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Enums;
using TaskManagement.Domain.Interfaces;
using TaskManagement.Domain.Models;
using TaskManagement.Infrastructure.Persistence;

namespace TaskManagement.Infrastructure.Repositories;

/// <summary>Task-specific queries: soft-delete filtering, navigations, search/filter/sort/paging.</summary>
public class TaskRepository : Repository<TaskItem>, ITaskRepository
{
    public TaskRepository(AppDbContext context) : base(context)
    {
    }

    /// <summary>Fetches a non-deleted task with its assignee/creator loaded.</summary>
    public override async Task<TaskItem?> GetByIdAsync(int id) =>
        await DbSet.Include(t => t.AssignedToUser)
                    .Include(t => t.CreatedByUser)
                    .FirstOrDefaultAsync(t => t.TaskId == id && !t.IsDeleted);

    /// <summary>Fetches every non-deleted task with its assignee/creator loaded.</summary>
    public override async Task<IReadOnlyList<TaskItem>> GetAllAsync() =>
        await DbSet.Include(t => t.AssignedToUser)
                    .Include(t => t.CreatedByUser)
                    .Where(t => !t.IsDeleted)
                    .ToListAsync();

    /// <summary>Returns one page of non-deleted tasks plus the total matching count.</summary>
    public async Task<(IReadOnlyList<TaskItem> Items, int TotalCount)> SearchAsync(
        string? search,
        TaskItemStatus? status,
        TaskPriority? priority,
        string? sortBy,
        bool sortDescending,
        int page,
        int pageSize)
    {
        // Filters are applied to a bare query first: COUNT(*) shouldn't drag the
        // AssignedToUser/CreatedByUser joins along with it.
        var query = DbSet.Where(t => !t.IsDeleted);

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(t => t.Title.Contains(search) ||
                                      (t.Description != null && t.Description.Contains(search)));
        }

        if (status.HasValue)
        {
            query = query.Where(t => t.Status == status.Value);
        }

        if (priority.HasValue)
        {
            query = query.Where(t => t.Priority == priority.Value);
        }

        var totalCount = await query.CountAsync();

        var paged = query.Include(t => t.AssignedToUser)
                         .Include(t => t.CreatedByUser);

        var sorted = sortBy?.Trim().ToLower() switch
        {
            "duedate" => sortDescending ? paged.OrderByDescending(t => t.DueDate) : paged.OrderBy(t => t.DueDate),
            "priority" => sortDescending ? paged.OrderByDescending(t => t.Priority) : paged.OrderBy(t => t.Priority),
            "status" => sortDescending ? paged.OrderByDescending(t => t.Status) : paged.OrderBy(t => t.Status),
            "title" => sortDescending ? paged.OrderByDescending(t => t.Title) : paged.OrderBy(t => t.Title),
            _ => paged.OrderByDescending(t => t.CreatedDate)
        };

        // ThenBy(TaskId) keeps paging deterministic when the sort column has ties -
        // without it a row can appear on two pages or none.
        var items = await sorted.ThenBy(t => t.TaskId)
                                .Skip((page - 1) * pageSize)
                                .Take(pageSize)
                                .ToListAsync();

        return (items, totalCount);
    }

    /// <summary>Aggregates task counts by status in SQL, without loading any rows.</summary>
    public async Task<TaskStatistics> GetStatisticsAsync()
    {
        var query = DbSet.Where(t => !t.IsDeleted);

        // One GROUP BY round trip for the status buckets...
        var byStatus = await query
            .GroupBy(t => t.Status)
            .Select(g => new { Status = g.Key, Count = g.Count() })
            .ToListAsync();

        // ...and one indexed COUNT for overdue (a date comparison, not a status).
        var today = DateTime.UtcNow.Date;
        var overdue = await query.CountAsync(t =>
            t.Status != TaskItemStatus.Completed &&
            t.Status != TaskItemStatus.Cancelled &&
            t.DueDate < today);

        int CountFor(TaskItemStatus status) =>
            byStatus.FirstOrDefault(x => x.Status == status)?.Count ?? 0;

        return new TaskStatistics(
            Total: byStatus.Sum(x => x.Count),
            ToDo: CountFor(TaskItemStatus.ToDo),
            InProgress: CountFor(TaskItemStatus.InProgress),
            Completed: CountFor(TaskItemStatus.Completed),
            Cancelled: CountFor(TaskItemStatus.Cancelled),
            Overdue: overdue);
    }
}
