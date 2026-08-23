namespace Catchen.Identity.Models;

/// <summary>
/// A registered account. Identity is a verified email or an E.164 telephone
/// number outside Mainland China (+86 is rejected before any account exists).
/// </summary>
public sealed class AppUser
{
    public Guid Id { get; set; }

    /// <summary>Lower-cased email address; unique across accounts.</summary>
    public required string Email { get; set; }

    /// <summary>Normalized E.164 telephone, or null when identity is email-only.</summary>
    public string? PhoneE164 { get; set; }

    /// <summary>PBKDF2 hash in versioned storage format — never a plaintext.</summary>
    public required string PasswordHash { get; set; }

    /// <summary>"RegularUser" or "Administrator".</summary>
    public required string Role { get; set; }

    public DateTimeOffset CreatedAtUtc { get; set; }
}

public static class AppUserRoles
{
    public const string RegularUser = "RegularUser";
    public const string Administrator = "Administrator";
}
