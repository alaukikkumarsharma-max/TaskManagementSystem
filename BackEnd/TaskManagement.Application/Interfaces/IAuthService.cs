using TaskManagement.Application.DTOs.Auth;
using TaskManagement.Application.DTOs.Roles;

namespace TaskManagement.Application.Interfaces;

/// <summary>Handles login, registration, and the roles lookup used by both.</summary>
public interface IAuthService
{
    /// <summary>Verifies credentials and issues a JWT.</summary>
    Task<AuthResponseDto> LoginAsync(LoginDto dto);

    /// <summary>Creates a new user account and issues a JWT.</summary>
    Task<AuthResponseDto> RegisterAsync(RegisterDto dto);

    /// <summary>Lists roles available for the registration form.</summary>
    Task<IReadOnlyList<RoleDto>> GetRolesAsync();
}
