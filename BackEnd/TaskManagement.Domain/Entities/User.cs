namespace TaskManagement.Domain.Entities;

/// <summary>An account that can sign in and be assigned tasks.</summary>
public class User
{
    // Primary key.
    public int UserId { get; set; }
    // Given name.
    public string FirstName { get; set; } = string.Empty;
    // Family name.
    public string LastName { get; set; } = string.Empty;
    // Login/contact email, unique.
    public string Email { get; set; } = string.Empty;
    // Hashed password (never plain text).
    public string PasswordHash { get; set; } = string.Empty;
    // Foreign key to the assigned Role.
    public int RoleId { get; set; }
    // False for deactivated accounts (excluded from login/lookups).
    public bool IsActive { get; set; } = true;
    // Timestamp the account was created.
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    // Timestamp of the last change.
    public DateTime UpdatedDate { get; set; } = DateTime.UtcNow;

    // Navigation to the assigned Role.
    public Role Role { get; set; } = null!;
    // Tasks assigned to this user.
    public ICollection<TaskItem> AssignedTasks { get; set; } = new List<TaskItem>();
    // Tasks this user created.
    public ICollection<TaskItem> CreatedTasks { get; set; } = new List<TaskItem>();
}
