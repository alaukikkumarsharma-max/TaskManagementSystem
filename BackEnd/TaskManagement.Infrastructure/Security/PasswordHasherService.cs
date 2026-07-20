using Microsoft.AspNetCore.Identity;
using TaskManagement.Application.Interfaces;
using TaskManagement.Domain.Entities;

namespace TaskManagement.Infrastructure.Security;

// Uses PasswordHasher<T> directly rather than pulling in full ASP.NET Core
// Identity (UserManager/SignInManager/IdentityDbContext) - this assessment
// only asks for "Authentication (Simple)", so this gets a well-tested,
// production-grade hasher (PBKDF2 under the hood) without that overhead.
/// <summary>Hashes and verifies passwords using ASP.NET Core's PasswordHasher.</summary>
public class PasswordHasherService : IPasswordHasher
{
    private readonly PasswordHasher<User> _hasher = new();

    // The PasswordHasher<T> API takes a TUser instance, but the default
    // implementation never actually reads it - passing null! here is the
    // standard pattern when using this hasher outside full Identity.
    /// <summary>Produces a salted hash for a plain-text password.</summary>
    public string HashPassword(string password) => _hasher.HashPassword(null!, password);

    /// <summary>Checks a plain-text password against a stored hash.</summary>
    public bool VerifyPassword(string hashedPassword, string providedPassword)
    {
        var result = _hasher.VerifyHashedPassword(null!, hashedPassword, providedPassword);
        return result is PasswordVerificationResult.Success or PasswordVerificationResult.SuccessRehashNeeded;
    }
}
