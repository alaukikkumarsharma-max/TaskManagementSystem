namespace TaskManagement.Application.DTOs.Users;

/// <summary>A user option for the task assignee dropdown.</summary>
public class UserDto
{
    // Primary key.
    public int UserId { get; set; }
    // Display name.
    public string FullName { get; set; } = string.Empty;
    // Contact email.
    public string Email { get; set; } = string.Empty;
    // Role display name.
    public string RoleName { get; set; } = string.Empty;
}
