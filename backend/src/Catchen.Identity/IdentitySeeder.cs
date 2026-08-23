using Catchen.Identity.Models;
using Catchen.Identity.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Catchen.Identity;

/// <summary>
/// Seeds the initial administrator when the users table is empty.
/// Production MUST provide Seed:AdminEmail / Seed:AdminPassword (≥12 chars);
/// Development falls back to local-only defaults.
/// </summary>
public static class IdentitySeeder
{
    public static async Task EnsureAdminAsync(
        DbContext db,
        IConfiguration configuration,
        IPasswordHasher passwordHasher,
        ILogger logger,
        CancellationToken cancellationToken = default)
    {
        var anyUser = await db.Set<AppUser>().AnyAsync(cancellationToken);
        if (anyUser)
        {
            return;
        }

        var email = configuration["Seed:AdminEmail"];
        var password = configuration["Seed:AdminPassword"];

        var isDevelopment = string.Equals(
            configuration["ASPNETCORE_ENVIRONMENT"], "Development", StringComparison.OrdinalIgnoreCase);

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            if (!isDevelopment)
            {
                throw new InvalidOperationException(
                    "Production startup requires Seed:AdminEmail and Seed:AdminPassword "
                    + "(set Seed__AdminEmail / Seed__AdminPassword).");
            }

            // Local-development bootstrap credentials only; never used when
            // Seed:Admin* is configured (mandatory outside Development).
#pragma warning disable S2068
            email = "admin@catchen.local";
            password = "Admin#Local-2026";
#pragma warning restore S2068
        }

        if (password.Length < 12)
        {
            throw new InvalidOperationException("Seed:AdminPassword must be at least 12 characters.");
        }

        db.Set<AppUser>().Add(new AppUser
        {
            Id = Guid.NewGuid(),
            Email = email.Trim().ToLowerInvariant(),
            PhoneE164 = null,
            PasswordHash = passwordHasher.Hash(password),
            Role = AppUserRoles.Administrator,
            CreatedAtUtc = DateTimeOffset.UtcNow,
        });

        await db.SaveChangesAsync(cancellationToken);

        logger.LogSeededAdmin(email);
    }
}

internal static partial class SeederLogging
{
    [LoggerMessage(Level = LogLevel.Information, Message = "Seeded initial administrator {AdminEmail}")]
    public static partial void LogSeededAdmin(this ILogger logger, string adminEmail);
}
