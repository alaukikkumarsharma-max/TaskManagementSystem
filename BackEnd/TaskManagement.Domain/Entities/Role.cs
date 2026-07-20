namespace TaskManagement.Domain.Entities;

/// <summary>A user role (e.g. Admin, Manager, Employee).</summary>
public class Role
{
    // Primary key.
    public int RoleId { get; set; }
    // Display/claim name of the role.
    public string Name { get; set; } = string.Empty;

    // Users assigned to this role.
    public ICollection<User> Users { get; set; } = new List<User>();
}
