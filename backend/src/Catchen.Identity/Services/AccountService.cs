using System.Text;
using System.Text.Json;
using Catchen.Identity.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Catchen.Identity.Services;

public sealed record RegistrationRequest(
    string Email,
    string Password,
    string? PhoneE164Raw,
    string? DeclaredCountryCode,
    string AgreementVersionAccepted,
    string? ClientIp,
    string? ClientUserAgent);

public sealed record RegistrationResult(Guid? UserId, IReadOnlyList<string> Violations)
{
    public bool Succeeded => UserId is not null;

    public static RegistrationResult Ok(Guid userId)
    {
        return new(userId, Array.Empty<string>());
    }

    public static RegistrationResult Rejected(IReadOnlyList<string> violations)
    {
        return new(null, violations);
    }
}

public sealed record AuthenticationResult(string? Token, DateTimeOffset? ExpiresAtUtc, Guid? UserId)
{
    public bool Succeeded => Token is not null;
}

/// <summary>
/// Account lifecycle: registration with region/phone/agreement enforcement
/// and password authentication. Every denial is audited with reason codes.
/// </summary>
public interface IAccountService
{
    Task<RegistrationResult> RegisterAsync(RegistrationRequest request, CancellationToken cancellationToken = default);

    Task<AuthenticationResult> AuthenticateAsync(string email, string password, CancellationToken cancellationToken = default);
}

public sealed class AccountService(
    DbContext db,
    IOptions<IdentityOptions> options,
    IPasswordHasher passwordHasher,
    IPhoneNumberPolicy phonePolicy,
    IRegionPolicyService regionPolicy,
    IJwtTokenService jwtTokens,
    IAuditWriter audit,
    TimeProvider clock) : IAccountService
{
    public async Task<RegistrationResult> RegisterAsync(RegistrationRequest request, CancellationToken cancellationToken = default)
    {
        var violations = new List<string>();

        var email = request.Email?.Trim().ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(email) || !IsValidEmail(email))
        {
            violations.Add("email_invalid");
        }

        if (string.IsNullOrWhiteSpace(request.Password) || request.Password.Length < 10)
        {
            violations.Add("password_too_short");
        }

        if (!string.Equals(request.AgreementVersionAccepted, options.Value.RequiredAgreementVersion, StringComparison.Ordinal))
        {
            violations.Add("agreement_version_not_accepted");
        }

        string? phone = null;
        if (!string.IsNullOrWhiteSpace(request.PhoneE164Raw))
        {
            phone = phonePolicy.Normalize(request.PhoneE164Raw);
            if (phone is null)
            {
                violations.Add("phone_invalid");
            }
            // Country policy (+86 rejection) is owned by IRegionPolicyService
            // so registration and every other boundary share one decision.
        }

        var region = regionPolicy.Evaluate(new RegionSignals(phone, request.DeclaredCountryCode));
        violations.AddRange(region.Violations);

        if (violations.Count > 0)
        {
            await audit.WriteAsync(
                "identity", "registration.rejected", null,
                "AppUser", AuditEvidence.HashIp(request.ClientIp),
                new { violations },
                cancellationToken);

            return RegistrationResult.Rejected(violations);
        }

        var emailTaken = await db.Set<AppUser>().AnyAsync(u => u.Email == email, cancellationToken);
        if (emailTaken)
        {
            // Do not reveal account existence; audit the collision for ops.
            await audit.WriteAsync(
                "identity", "registration.email_collision", null,
                "AppUser", AuditEvidence.HashIp(request.ClientIp),
                new { email_digest = Digest(email!) },
                cancellationToken);

            return RegistrationResult.Rejected(["email_unavailable"]);
        }

        var user = new AppUser
        {
            Id = Guid.NewGuid(),
            Email = email!,
            PhoneE164 = phone,
            PasswordHash = passwordHasher.Hash(request.Password),
            Role = AppUserRoles.RegularUser,
            CreatedAtUtc = clock.GetUtcNow(),
        };

        db.Set<AppUser>().Add(user);
        db.Set<AgreementAcceptance>().Add(new AgreementAcceptance
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            AgreementVersion = request.AgreementVersionAccepted,
            AcceptedAtUtc = clock.GetUtcNow(),
            ClientIpHash = AuditEvidence.HashIp(request.ClientIp),
            ClientUserAgent = AuditEvidence.TruncateUserAgent(request.ClientUserAgent),
        });

        await db.SaveChangesAsync(cancellationToken);

        await audit.WriteAsync(
            "identity", "registration.completed", user.Id,
            "AppUser", user.Id.ToString(),
            new { identity = phone is null ? "email" : "email+phone" },
            cancellationToken);

        return RegistrationResult.Ok(user.Id);
    }

    public async Task<AuthenticationResult> AuthenticateAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        var normalized = email?.Trim().ToLowerInvariant();
        var user = string.IsNullOrWhiteSpace(normalized)
            ? null
            : await db.Set<AppUser>().SingleOrDefaultAsync(u => u.Email == normalized, cancellationToken);

        if (user is null || !passwordHasher.Verify(password ?? string.Empty, user.PasswordHash))
        {
            return new AuthenticationResult(null, null, null);
        }

        var (token, expires) = jwtTokens.Issue(user);
        return new AuthenticationResult(token, expires, user.Id);
    }

    private static bool IsValidEmail(string email)
    {
        // Deliberately conservative: one '@', non-empty local and domain parts.
        return email.Count(c => c == '@') == 1
        && email.Split('@') is { Length: 2 } parts
        && parts[0].Length > 0 && parts[1].Contains('.') && parts[1].Length > 2;
    }

    private static string Digest(string value)
    {
        return Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(Encoding.UTF8.GetBytes(value))).ToLowerInvariant();
    }
}
