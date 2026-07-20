using TaskManagement.Domain.Entities;

namespace TaskManagement.Application.Interfaces;

/// <summary>Issues signed JWTs for authenticated users.</summary>
public interface IJwtTokenGenerator
{
    /// <summary>Builds a signed token and its expiry for the given user.</summary>
    (string Token, DateTime ExpiresAtUtc) GenerateToken(User user);
}
