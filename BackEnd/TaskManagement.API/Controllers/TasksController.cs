using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.Application.DTOs.Common;
using TaskManagement.Application.DTOs.Tasks;
using TaskManagement.Application.DTOs.Users;
using TaskManagement.Application.Interfaces;
using TaskManagement.Domain.Enums;

namespace TaskManagement.API.Controllers;

/// <summary>CRUD for tasks; create/update/delete are restricted to Admin/Manager.</summary>
[ApiController]
[Authorize]
[Route("api/[controller]")]
public class TasksController : ControllerBase
{
    private readonly ITaskService _taskService;

    public TasksController(ITaskService taskService)
    {
        _taskService = taskService;
    }

    // GET /api/tasks?search=&status=ToDo&priority=High&sortBy=dueDate|title&sortDescending=&page=1&pageSize=20
    /// <summary>Returns one page of tasks after search, status/priority filters, and sorting.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<TaskDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<TaskDto>>> GetTasks(
        [FromQuery] string? search,
        [FromQuery] TaskItemStatus? status,
        [FromQuery] TaskPriority? priority,
        [FromQuery] string? sortBy,
        [FromQuery] bool sortDescending = false,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var tasks = await _taskService.GetTasksAsync(search, status, priority, sortBy, sortDescending, page, pageSize);
        return Ok(tasks);
    }
    /// <summary>Returns dashboard counters (totals by status, plus overdue).</summary>
    [HttpGet("stats")]
    [ProducesResponseType(typeof(TaskStatsDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<TaskStatsDto>> GetStats()
    {
        var stats = await _taskService.GetStatsAsync();
        return Ok(stats);
    }

    /// <summary>Fetches a single task by id.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(TaskDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TaskDto>> GetTask(int id)
    {
        var task = await _taskService.GetTaskByIdAsync(id);
        return Ok(task);
    }

    // Backs the "assign to" dropdown on the create/edit task form - any
    // authenticated role can read it (Employees see who a task is assigned
    // to; only Admin/Manager can actually assign, enforced by the
    // [Authorize] on CreateTask/UpdateTask below).
    /// <summary>Lists users eligible to be assigned a task.</summary>
    [HttpGet("assignable-users")]
    [ProducesResponseType(typeof(IReadOnlyList<UserDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<UserDto>>> GetAssignableUsers()
    {
        var users = await _taskService.GetAssignableUsersAsync();
        return Ok(users);
    }

    // Only Admin/Manager can create, edit, or delete tasks - Employees are
    // read-only here (view via GetTasks/GetTask, which stay open to any role).
    /// <summary>Creates a new task. Admin/Manager only.</summary>
    [Authorize(Roles = "Admin,Manager")]
    [HttpPost]
    [ProducesResponseType(typeof(TaskDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TaskDto>> CreateTask([FromBody] CreateTaskDto dto)
    {
        var createdByUserId = GetCurrentUserId();
        var created = await _taskService.CreateTaskAsync(dto, createdByUserId);
        return CreatedAtAction(nameof(GetTask), new { id = created.TaskId }, created);
    }

    /// <summary>Updates an existing task. Admin/Manager only.</summary>
    [Authorize(Roles = "Admin,Manager")]
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(TaskDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TaskDto>> UpdateTask(int id, [FromBody] UpdateTaskDto dto)
    {
        var userId = GetCurrentUserId();
        var updated = await _taskService.UpdateTaskAsync(id, dto, userId);
        return Ok(updated);
    }

    // Soft delete - see TaskService.DeleteTaskAsync.
    /// <summary>Soft-deletes a task. Admin/Manager only.</summary>
    [Authorize(Roles = "Admin,Manager")]
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteTask(int id)
    {
        var deletedByUserId = GetCurrentUserId();
        await _taskService.DeleteTaskAsync(id, deletedByUserId);
        return NoContent();
    }

    /// <summary>Reads the current user's id from the JWT claims.</summary>
    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new InvalidOperationException("Authenticated request is missing a user id claim.");

        return int.Parse(userIdClaim);
    }
}
