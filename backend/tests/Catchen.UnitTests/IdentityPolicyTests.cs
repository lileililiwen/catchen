using Catchen.Identity;
using Catchen.Identity.Services;
using Xunit;

namespace Catchen.UnitTests;

public sealed class PhoneNumberPolicyTests
{
    private readonly PhoneNumberPolicy _policy = new();

    [Theory]
    [InlineData("+44 7700 900123", "+447700900123")]
    [InlineData("0044-7700-900123", "+447700900123")]
    [InlineData("+1 (415) 555-0100", "+14155550100")]
    public void Normalizes_common_formats_to_e164(string raw, string expected)
    {
        Assert.Equal(expected, _policy.Normalize(raw));
    }

    [Theory]
    [InlineData("07700900123")]          // no country code
    [InlineData("+0123456")]             // CC cannot start with 0
    [InlineData("+44abc123")]            // non-digit
    [InlineData("+1")]                   // too short
    public void Rejects_unparseable_numbers(string raw)
    {
        Assert.Null(_policy.Normalize(raw));
    }

    [Fact]
    public void Mainland_china_numbers_are_detected_by_prefix()
    {
        Assert.True(_policy.IsMainlandChina(_policy.Normalize("+8613800138000")!));
        Assert.False(_policy.IsMainlandChina(_policy.Normalize("+85298765432")!));
        Assert.False(_policy.IsMainlandChina(_policy.Normalize("+88623456789")!)); // Taiwan +886
    }
}

public sealed class RegionPolicyServiceTests
{
    private static RegionPolicyService Service()
    {
        return new(Microsoft.Extensions.Options.Options.Create(new IdentityOptions
        {
            BlockedPhoneCountryCodes = PolicyDefaults.BlockedPhoneCountryCodes,
            BlockedDeclaredCountries = PolicyDefaults.BlockedDeclaredCountries,
        }));
    }

    [Fact]
    public void Overseas_signals_are_allowed()
    {
        var decision = Service().Evaluate(new RegionSignals("+447700900123", "GB"));

        Assert.True(decision.IsAllowed);
        Assert.Empty(decision.Violations);
    }

    [Fact]
    public void Mainland_phone_is_blocked_even_without_declared_country()
    {
        var decision = Service().Evaluate(new RegionSignals("+8613800138000", null));

        Assert.False(decision.IsAllowed);
        Assert.Contains("phone_country_blocked:+86", decision.Violations);
    }

    [Fact]
    public void Declared_mainland_country_is_blocked()
    {
        var decision = Service().Evaluate(new RegionSignals(null, "CN"));

        Assert.False(decision.IsAllowed);
        Assert.Contains("declared_country_blocked:CN", decision.Violations);
    }

    [Fact]
    public void Violations_are_aggregated_and_reason_coded()
    {
        var decision = Service().Evaluate(new RegionSignals("+8613800138000", "CN"));

        Assert.Equal(2, decision.Violations.Count);
    }
}

public sealed class PasswordHasherTests
{
    private readonly Pbdkf2PasswordHasher _hasher = new();

    [Fact]
    public void Hash_verifies_the_correct_password()
    {
        var hash = _hasher.Hash("Passw0rd!long");

        Assert.True(_hasher.Verify("Passw0rd!long", hash));
        Assert.False(_hasher.Verify("wrong-password", hash));
    }

    [Fact]
    public void Salts_are_unique_per_hash()
    {
        Assert.NotEqual(_hasher.Hash("same"), _hasher.Hash("same"));
    }

    [Fact]
    public void Malformed_stored_hashes_are_rejected_not_thrown()
    {
        Assert.False(_hasher.Verify("x", "not-a-hash"));
    }
}
