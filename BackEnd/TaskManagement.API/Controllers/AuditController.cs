using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.Application.DTOs.Audit;
using TaskManagement.Application.Interfaces;

namespace TaskManagement.API.Controllers;

/// <summary>Read-only access to the audit trail. Admin only — this is oversight data.</summary>
[ApiController]
[Authorize(Roles = "Admin")]
[Route("api/[controller]")]
public class AuditController : ControllerBase
{
    private readonly IAuditService _auditService;

    public AuditController(IAuditService auditService)
    {
        _auditService = auditService;
    }

    /// <summary>Lists the most recent audit entries (default 100, capped at 500).</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<AuditLogDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<AuditLogDto>>> GetRecent([FromQuery] int take = 100)
    {
        var logs = await _auditService.GetRecentAsync(Math.Clamp(take, 1, 500));
        return Ok(logs);
    }
}
