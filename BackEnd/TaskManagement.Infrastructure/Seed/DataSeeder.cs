using Microsoft.EntityFrameworkCore;
using TaskManagement.Application.Interfaces;
using TaskManagement.Infrastructure.Persistence;

namespace TaskManagement.Infrastructure.Seed;

// TaskManagement_Database_Schema.sql seeds three users with the literal
// string "REPLACE_WITH_REAL_HASH" in PasswordHash, because a real hash can
// only be produced by the hasher this API actually uses. On startup (dev
// only - see Program.cs), this swaps those placeholders for a real hash of
// "Passw0rd!" so the seeded accounts can log in immediately. Once the
// placeholder is gone this is a no-op, so it's safe to leave wired in.
/// <summary>Dev-only fixup that replaces seeded placeholder password hashes with real ones.</summary>
public static class DataSeeder
{
    // Placeholder value written by the SQL seed script.
    private const string PlaceholderHash = "REPLACE_WITH_REAL_HASH";
    // Password every seeded account gets once fixed up.
    private const string DefaultSeedPassword = "Passw0rd!";

    /// <summary>Replaces any placeholder password hashes still present in the Users table.</summary>
    public static async Task FixSeedPasswordsAsync(AppDbContext context, IPasswordHasher passwordHasher)
    {
        var usersToFix = await context.Users
            .Where(u => u.PasswordHash == PlaceholderHash)
            .ToListAsync();

        if (usersToFix.Count == 0)
        {
            return;
        }

        foreach (var user in usersToFix)
        {
            user.PasswordHash = passwordHasher.HashPassword(DefaultSeedPassword);
        }

        await context.SaveChangesAsync();
    }
}
