using Microsoft.Extensions.Options;

namespace Catchen.Identity.Services;

public sealed record RegionSignals(string? PhoneE164, string? DeclaredCountryCode);

/// <summary>Reason-coded decision; never a bare boolean at the boundary.</summary>
public sealed record RegionDecision(bool IsAllowed, IReadOnlyList<string> Violations)
{
    public static readonly RegionDecision Allow = new(true, Array.Empty<string>());
}

/// <summary>
/// Evaluates whether a person may be served, combining the declared country
/// and the telephone country code. Signals are layered — no single signal is
/// treated as proof of location (VPN-aware by design); decisions are always
/// reason-coded so denials can be audited without storing raw personal data.
/// </summary>
public interface IRegionPolicyService
{
    RegionDecision Evaluate(RegionSignals signals);
}

public sealed class RegionPolicyService : IRegionPolicyService
{
    private readonly IdentityOptions _options;

    public RegionPolicyService(IOptions<IdentityOptions> options)
    {
        _options = options.Value;
    }

    public RegionDecision Evaluate(RegionSignals signals)
    {
        var violations = new List<string>();

        if (!string.IsNullOrWhiteSpace(signals.DeclaredCountryCode))
        {
            var declared = signals.DeclaredCountryCode.Trim().ToUpperInvariant();
            if (_options.BlockedDeclaredCountries.Contains(declared, StringComparer.Ordinal))
            {
                violations.Add($"declared_country_blocked:{declared}");
            }
        }

        if (!string.IsNullOrWhiteSpace(signals.PhoneE164))
        {
            // Blocked codes are matched as E.164 PREFIXES: any number starting
            // with a blocked country code (e.g. "+86") is refused. Prefix
            // matching avoids mis-parsing unallocated 2-3 digit combinations
            // (e.g. reading "+861…" as the non-existent code "+861").
            var blocked = _options.BlockedPhoneCountryCodes.FirstOrDefault(code =>
                signals.PhoneE164.StartsWith(code, StringComparison.Ordinal));

            if (blocked is not null)
            {
                violations.Add($"phone_country_blocked:{blocked}");
            }
        }

        return violations.Count == 0 ? RegionDecision.Allow : new RegionDecision(false, violations);
    }
}
