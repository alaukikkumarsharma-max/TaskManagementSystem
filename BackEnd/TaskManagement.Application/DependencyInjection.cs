using Microsoft.Extensions.DependencyInjection;
using TaskManagement.Application.Interfaces;
using TaskManagement.Application.Services;

namespace TaskManagement.Application;

/// <summary>Registers Application-layer services with the DI container.</summary>
public static class DependencyInjection
{
    /// <summary>Adds all Application services as scoped dependencies.</summary>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ITaskService, TaskService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IAuditService, AuditService>();

        return services;
    }
}
