namespace TaskManagement.Application.Interfaces;

/// <summary>Hashes and verifies passwords.</summary>
public interface IPasswordHasher
{
    /// <summary>Produces a salted hash for a plain-text password.</summary>
    string HashPassword(string password);

    /// <summary>Checks a plain-text password against a stored hash.</summary>
    bool VerifyPassword(string hashedPassword, string providedPassword);
}
