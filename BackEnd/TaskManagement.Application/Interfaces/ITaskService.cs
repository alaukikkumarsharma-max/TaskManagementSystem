using TaskManagement.Application.DTOs.Common;
using TaskManagement.Application.DTOs.Tasks;
using TaskManagement.Application.DTOs.Users;
using TaskManagement.Domain.Enums;

namespace TaskManagement.Application.Interfaces;

/// <summary>CRUD operations and lookups for tasks.</summary>
public interface ITaskService
{
    /// <summary>Returns one page of tasks after applying search, filters, and sorting.</summary>
    Task<PagedResult<TaskDto>> GetTasksAsync(string? search, TaskItemStatus? status, TaskPriority? priority, string? sortBy, bool sortDescending, int page, int pageSize);

    /// <summary>Returns dashboard counters aggregated in SQL.</summary>
    Task<TaskStatsDto> GetStatsAsync();

    /// <summary>Fetches a single task by id.</summary>
    Task<TaskDto> GetTaskByIdAsync(int id);

    /// <summary>Creates a new task on behalf of the given user.</summary>
    Task<TaskDto> CreateTaskAsync(CreateTaskDto dto, int userId);

    /// <summary>Updates an existing task on behalf of the given user.</summary>
    Task<TaskDto> UpdateTaskAsync(int id, UpdateTaskDto dto, int userId);

    /// <summary>Soft-deletes a task on behalf of the given user.</summary>
    Task DeleteTaskAsync(int id, int userId);

    // Backs the task-form's "assign to" dropdown - lives here rather than a
    // dedicated Users feature because tasks are the only consumer.
    /// <summary>Lists users eligible to be assigned a task.</summary>
    Task<IReadOnlyList<UserDto>> GetAssignableUsersAsync();
}
