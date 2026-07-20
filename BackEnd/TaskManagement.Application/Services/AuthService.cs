using TaskManagement.Application.DTOs.Auth;
using TaskManagement.Application.DTOs.Roles;
using TaskManagement.Application.Exceptions;
using TaskManagement.Application.Interfaces;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Enums;
using TaskManagement.Domain.Interfaces;

namespace TaskManagement.Application.Services;

/// <summary>Handles login, registration, and role lookups.</summary>
public class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _tokenGenerator;
    private readonly IAuditService _auditService;

    public AuthService(
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator tokenGenerator,
        IAuditService auditService)
    {
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _tokenGenerator = tokenGenerator;
        _auditService = auditService;
    }

    /// <summary>Verifies credentials, logs the event, and issues a JWT.</summary>
    public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
    {
        var user = await _unitOfWork.Users.GetByEmailAsync(dto.Email)
            ?? throw new UnauthorizedException("Invalid email or password.");

        if (!_passwordHasher.VerifyPassword(user.PasswordHash, dto.Password))
        {
            throw new UnauthorizedException("Invalid email or password.");
        }

        var (token, expiresAtUtc) = _tokenGenerator.GenerateToken(user);

        await _auditService.LogAsync(user.UserId, AuditAction.Login);

        return new AuthResponseDto
        {
            Token = token,
            ExpiresAtUtc = expiresAtUtc,
            UserId = user.UserId,
            FullName = $"{user.FirstName} {user.LastName}",
            Email = user.Email,
            Role = user.Role.Name
        };
    }

    /// <summary>Creates a new user account and issues a JWT.</summary>
    public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
    {
        var existingUser = await _unitOfWork.Users.GetByEmailAsync(dto.Email);
        if (existingUser is not null)
        {
            throw new BadRequestException("A user with this email already exists.");
        }

        var user = new User
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            PasswordHash = _passwordHasher.HashPassword(dto.Password),
            RoleId = dto.RoleId
        };

        await _unitOfWork.Users.AddAsync(user);
        await _unitOfWork.SaveChangesAsync();

        // Re-fetch with Role included - GetByEmailAsync is the only query that eager-loads it.
        var savedUser = await _unitOfWork.Users.GetByEmailAsync(user.Email)
            ?? throw new NotFoundException("User", user.UserId);

        var (token, expiresAtUtc) = _tokenGenerator.GenerateToken(savedUser);

        return new AuthResponseDto
        {
            Token = token,
            ExpiresAtUtc = expiresAtUtc,
            UserId = savedUser.UserId,
            FullName = $"{savedUser.FirstName} {savedUser.LastName}",
            Email = savedUser.Email,
            Role = savedUser.Role.Name
        };
    }

    /// <summary>Lists roles available for the registration form.</summary>
    public async Task<IReadOnlyList<RoleDto>> GetRolesAsync()
    {
        var roles = await _unitOfWork.Roles.GetAllAsync();

        return roles
            .OrderBy(r => r.Name)
            .Select(r => new RoleDto { RoleId = r.RoleId, Name = r.Name })
            .ToList();
    }
}
