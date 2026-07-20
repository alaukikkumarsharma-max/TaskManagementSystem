using System.ComponentModel.DataAnnotations;

namespace TaskManagement.Application.DTOs.Auth;

/// <summary>Credentials submitted to the login endpoint.</summary>
public class LoginDto
{
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress]
    // Account email.
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required.")]
    // Plain-text password (hashed/verified server-side).
    public string Password { get; set; } = string.Empty;
}

/// <summary>New account details submitted to the register endpoint.</summary>
public class RegisterDto
{
    [Required(ErrorMessage = "First name is required.")]
    // Given name.
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Last name is required.")]
    // Family name.
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress]
    // Login email, must be unique.
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required.")]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters.")]
    // Plain-text password (hashed before storage).
    public string Password { get; set; } = string.Empty;

    // Must reference an existing row in the Roles table (see TaskManagement_Database_Schema.sql seed data).
    [Required(ErrorMessage = "RoleId is required.")]
    public int RoleId { get; set; }
}

/// <summary>Returned after a successful login or registration.</summary>
public class AuthResponseDto
{
    // Signed JWT for subsequent requests.
    public string Token { get; set; } = string.Empty;
    // UTC expiry of the token.
    public DateTime ExpiresAtUtc { get; set; }
    // Id of the authenticated user.
    public int UserId { get; set; }
    // Display name of the authenticated user.
    public string FullName { get; set; } = string.Empty;
    // Email of the authenticated user.
    public string Email { get; set; } = string.Empty;
    // Role name of the authenticated user.
    public string Role { get; set; } = string.Empty;
}
