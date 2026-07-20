namespace TaskManagement.Application.DTOs.Roles;

/// <summary>A role option for the registration form.</summary>
public class RoleDto
{
    // Primary key.
    public int RoleId { get; set; }
    // Display name.
    public string Name { get; set; } = string.Empty;
}
