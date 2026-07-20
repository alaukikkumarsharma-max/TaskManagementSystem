namespace TaskManagement.Application.Settings;

/// <summary>Bound from the "JwtSettings" configuration section.</summary>
public class JwtSettings
{
    // Configuration section name used for binding.
    public const string SectionName = "JwtSettings";

    // Symmetric signing key.
    public string Secret { get; set; } = string.Empty;
    // Expected token issuer.
    public string Issuer { get; set; } = string.Empty;
    // Expected token audience.
    public string Audience { get; set; } = string.Empty;
    // Token lifetime in minutes.
    public int ExpiryMinutes { get; set; } = 60;
}
