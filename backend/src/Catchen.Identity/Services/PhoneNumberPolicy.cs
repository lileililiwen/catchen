using System.Globalization;

namespace Catchen.Identity.Services;

/// <summary>
/// Normalizes and classifies telephone numbers in E.164 form. The only
/// policy-relevant classification is the country code: Mainland China (+86)
/// numbers are rejected before any OTP is sent and before any account exists.
/// </summary>
public interface IPhoneNumberPolicy
{
    /// <summary>Returns the normalized E.164 number, or null when unparseable.</summary>
    string? Normalize(string? input);

    bool IsMainlandChina(string normalizedE164);

    /// <summary>Country code including "+", e.g. "+44"; null when unparseable.</summary>
    string? CountryCodeOf(string normalizedE164);
}

public sealed class PhoneNumberPolicy : IPhoneNumberPolicy
{
    public string? Normalize(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return null;
        }

        var candidate = input.Trim()
            .Replace(" ", string.Empty)
            .Replace("-", string.Empty)
            .Replace("(", string.Empty)
            .Replace(")", string.Empty);

        if (candidate.StartsWith("00", StringComparison.Ordinal))
        {
            candidate = "+" + candidate[2..];
        }

        if (!candidate.StartsWith('+'))
        {
            return null;
        }

        var digits = candidate[1..];
        if (digits.Length is < 7 or > 15 || digits.Any(c => !char.IsDigit(c)))
        {
            return null;
        }

        // E.164 country codes never start with 0.
        if (digits[0] == '0')
        {
            return null;
        }

        return "+" + digits;
    }

    public bool IsMainlandChina(string normalizedE164)
    {
        return normalizedE164.StartsWith("+86", StringComparison.Ordinal);
    }

    public string? CountryCodeOf(string normalizedE164)
    {
        if (normalizedE164.Length < 4 || normalizedE164[0] != '+')
        {
            return null;
        }

        // Longest-match over the ITU country-code plan (1-3 digits).
        foreach (var length in new[] { 3, 2, 1 })
        {
            if (normalizedE164.Length >= 1 + length + 1)
            {
                var candidate = normalizedE164[..(1 + length)];
                if (candidate.All(c => char.IsDigit(c) || c == '+')
                    && int.TryParse(candidate.AsSpan(1), CultureInfo.InvariantCulture, out _))
                {
                    return candidate;
                }
            }
        }

        return null;
    }
}
