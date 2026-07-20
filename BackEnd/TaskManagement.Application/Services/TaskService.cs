using TaskManagement.Application.DTOs.Common;
using TaskManagement.Application.DTOs.Tasks;
using TaskManagement.Application.DTOs.Users;
using TaskManagement.Application.Exceptions;
using TaskManagement.Application.Interfaces;
using TaskManagement.Application.Mappings;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Enums;
using TaskManagement.Domain.Interfaces;

namespace TaskManagement.Application.Services;

/// <summary>CRUD operations and lookups for tasks.</summary>
public class TaskService : ITaskService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuditService _auditService;

    public TaskService(IUnitOfWork unitOfWork, IAuditService auditService)
    {
        _unitOfWork = unitOfWork;
        _auditService = auditService;
    }

    // Guard rails so a caller can't request page 0 or pull 1,000,000 rows in one page.
    private const int MaxPageSize = 100;
    private const int DefaultPageSize = 20;

    /// <summary>Returns one page of tasks after applying search, filters, and sorting.</summary>
    public async Task<PagedResult<TaskDto>> GetTasksAsync(
        string? search,
        TaskItemStatus? status,
        TaskPriority? priority,
        string? sortBy,
        bool sortDescending,
        int page,
        int pageSize)
    {
        page = page < 1 ? 1 : page;
        pageSize = pageSize < 1 ? DefaultPageSize : Math.Min(pageSize, MaxPageSize);

        var (items, totalCount) = await _unitOfWork.Tasks.SearchAsync(
            search, status, priority, sortBy, sortDescending, page, pageSize);

        return new PagedResult<TaskDto>
        {
            Items = items.Select(t => t.ToDto()).ToList(),
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    /// <summary>Returns dashboard counters aggregated in SQL.</summary>
    public async Task<TaskStatsDto> GetStatsAsync()
    {
        var stats = await _unitOfWork.Tasks.GetStatisticsAsync();

        return new TaskStatsDto
        {
            Total = stats.Total,
            ToDo = stats.ToDo,
            InProgress = stats.InProgress,
            Completed = stats.Completed,
            Cancelled = stats.Cancelled,
            Overdue = stats.Overdue
        };
    }

    /// <summary>Fetches a single task by id.</summary>
    public async Task<TaskDto> GetTaskByIdAsync(int id)
    {
        var task = await _unitOfWork.Tasks.GetByIdAsync(id)
            ?? throw new NotFoundException(nameof(TaskItem), id);

        return task.ToDto();
    }

    /// <summary>Creates a new task and records an audit entry.</summary>
    public async Task<TaskDto> CreateTaskAsync(CreateTaskDto dto, int userId)
    {
        // This is the ITaskRepository + IUserRepository pairing: creating a
        // task needs to check the *other* entity before it commits.
        await EnsureAssigneeIsValidAsync(dto.AssignedToUserId);

        var task = new TaskItem
        {
            Title = dto.Title,
            Description = dto.Description,
            Priority = dto.Priority,
            Status = TaskItemStatus.ToDo,
            DueDate = dto.DueDate,
            AssignedToUserId = dto.AssignedToUserId,
            CreatedByUserId = userId
        };

        task.EnsureDueDateNotInPast();

        await _unitOfWork.Tasks.AddAsync(task);
        await _unitOfWork.SaveChangesAsync();

        await _auditService.LogAsync(userId, AuditAction.TaskCreated, nameof(TaskItem), task.TaskId, task.Title);

        // Re-fetch through the repository so AssignedToUser/CreatedByUser
        // are loaded for the response DTO's AssignedToName/CreatedByName.
        var created = await _unitOfWork.Tasks.GetByIdAsync(task.TaskId)
            ?? throw new NotFoundException(nameof(TaskItem), task.TaskId);

        return created.ToDto();
    }

    /// <summary>Updates an existing task and records an audit entry.</summary>
    public async Task<TaskDto> UpdateTaskAsync(int id, UpdateTaskDto dto, int userId)
    {
        var task = await _unitOfWork.Tasks.GetByIdAsync(id)
            ?? throw new NotFoundException(nameof(TaskItem), id);

        await EnsureAssigneeIsValidAsync(dto.AssignedToUserId);

        task.Title = dto.Title;
        task.Description = dto.Description;
        task.Priority = dto.Priority;
        task.Status = dto.Status;
        task.DueDate = dto.DueDate;
        task.AssignedToUserId = dto.AssignedToUserId;
        task.EnsureDueDateNotInPast();
        task.MarkUpdated();

        _unitOfWork.Tasks.Update(task);
        await _unitOfWork.SaveChangesAsync();

        await _auditService.LogAsync(userId, AuditAction.TaskUpdated, nameof(TaskItem), task.TaskId, task.Title);

        return task.ToDto();
    }

    /// <summary>Soft-deletes a task and records an audit entry.</summary>
    public async Task DeleteTaskAsync(int id, int UserId)
    {
        var task = await _unitOfWork.Tasks.GetByIdAsync(id)
            ?? throw new NotFoundException(nameof(TaskItem), id);

        // Soft delete - IsDeleted is filtered out by TaskRepository's reads,
        // so this behaves like a delete everywhere except the DB itself.
        task.IsDeleted = true;
        task.MarkUpdated();

        _unitOfWork.Tasks.Update(task);
        await _unitOfWork.SaveChangesAsync();

        await _auditService.LogAsync(UserId, AuditAction.TaskDeleted, nameof(TaskItem), task.TaskId, task.Title);
    }

    /// <summary>Lists users eligible to be assigned a task.</summary>
    public async Task<IReadOnlyList<UserDto>> GetAssignableUsersAsync()
    {
        // UserRepository.GetAllAsync() includes Role and filters to active
        // users - that's exactly who's a valid assignee.
        var users = await _unitOfWork.Users.GetAllAsync();

        return users
            .OrderBy(u => u.FirstName)
            .ThenBy(u => u.LastName)
            .Select(u => new UserDto
            {
                UserId = u.UserId,
                FullName = $"{u.FirstName} {u.LastName}",
                Email = u.Email,
                RoleName = u.Role.Name
            })
            .ToList();
    }

    /// <summary>Throws if the given user id isn't an active user.</summary>
    private async Task EnsureAssigneeIsValidAsync(int? assignedToUserId)
    {
        if (assignedToUserId.HasValue && !await _unitOfWork.Users.ExistsAsync(assignedToUserId.Value))
        {
            throw new BadRequestException(
                $"AssignedToUserId '{assignedToUserId}' does not reference an active user.");
        }
    }
}
