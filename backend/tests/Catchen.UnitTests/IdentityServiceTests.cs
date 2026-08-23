using Catchen.Data;
using Catchen.Identity;
using Catchen.Identity.Models;
using Catchen.Identity.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Xunit;

namespace Catchen.UnitTests;

/// <summary>
/// Restricted-operation tests (task 1.3): registration/agreement/region
/// enforcement and channel-approval policy against a real relational store
/// (SQLite in-memory), exercising the same base-DbContext injection the
/// production modules use.
/// </summary>
public sealed class IdentityServiceTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly DbContext _db;
    private readonly IdentityOptions _identityOptions = new()
    {
        JwtSecret = "unit-test-secret-0123456789-0123456789",
        BlockedPhoneCountryCodes = PolicyDefaults.BlockedPhoneCountryCodes,
        BlockedDeclaredCountries = PolicyDefaults.BlockedDeclaredCountries,
    };
    private readonly ChannelPolicyOptions _channelOptions = new()
    {
        AllowedPaymentMethods = PolicyDefaults.AllowedPaymentMethods,
        ProhibitedPromotionChannels = PolicyDefaults.ProhibitedPromotionChannels,
        ProhibitedDistributionChannels = PolicyDefaults.ProhibitedDistributionChannels,
    };

    public IdentityServiceTests()
    {
        _connection = new SqliteConnection("Data Source=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<AppDbContext>().UseSqlite(_connection).Options;
        _db = new AppDbContext(options);
        _db.Database.EnsureCreated();
    }

    private AccountService CreateAccountService(FakeClock? clock = null)
    {
        var time = clock ?? new FakeClock();
        return new AccountService(
            _db,
            Microsoft.Extensions.Options.Options.Create(_identityOptions),
            new Pbdkf2PasswordHasher(),
            new PhoneNumberPolicy(),
            new RegionPolicyService(Microsoft.Extensions.Options.Options.Create(_identityOptions)),
            new JwtTokenService(Microsoft.Extensions.Options.Options.Create(_identityOptions), time),
            new AuditWriter(_db, time),
            time);
    }

    private ChannelPolicyService CreateChannelService(FakeClock? clock = null)
    {
        return new(
            _db,
            Microsoft.Extensions.Options.Options.Create(_channelOptions),
            new AuditWriter(_db, clock ?? new FakeClock()),
            clock ?? new FakeClock());
    }

    [Fact]
    public async Task Registration_persists_user_agreement_evidence_and_audit_event()
    {
        var service = CreateAccountService();

        var result = await service.RegisterAsync(new RegistrationRequest(
            "User@Example.com", "Passw0rd!long", null, "GB",
            "2026-08-consumer", "203.0.113.7", "test-agent"));

        Assert.True(result.Succeeded);

        var user = await _db.Set<AppUser>().SingleAsync();
        Assert.Equal("user@example.com", user.Email);
        Assert.Equal(AppUserRoles.RegularUser, user.Role);

        var acceptance = await _db.Set<AgreementAcceptance>().SingleAsync();
        Assert.Equal(user.Id, acceptance.UserId);
        Assert.Equal("2026-08-consumer", acceptance.AgreementVersion);
        Assert.Equal(64, acceptance.ClientIpHash.Length); // SHA-256 hex digest

        // Exactly one success event; denials are the only other audit source.
        Assert.Equal(1, await _db.Set<AuditEvent>().CountAsync());
    }

    [Fact]
    public async Task Mainland_phone_registration_is_rejected_before_account_creation()
    {
        var service = CreateAccountService();

        var result = await service.RegisterAsync(new RegistrationRequest(
            "cn@example.com", "Passw0rd!long", "+8613800138000", null,
            "2026-08-consumer", "198.51.100.9", "test-agent"));

        Assert.False(result.Succeeded);
        Assert.Contains("phone_country_blocked:+86", result.Violations);
        Assert.Empty(await _db.Set<AppUser>().ToListAsync());          // no account exists
        Assert.Single(await _db.Set<AuditEvent>().ToListAsync());      // rejection audited, reason-coded
    }

    [Fact]
    public async Task Duplicate_email_is_reported_without_leaking_existence()
    {
        var service = CreateAccountService();
        await service.RegisterAsync(new RegistrationRequest(
            "taken@example.com", "Passw0rd!long", null, "GB",
            "2026-08-consumer", null, null));

        var second = await service.RegisterAsync(new RegistrationRequest(
            "taken@example.com", "Another#Pass1", null, "GB",
            "2026-08-consumer", null, null));

        Assert.False(second.Succeeded);
        Assert.Equal(["email_unavailable"], second.Violations);
    }

    [Fact]
    public async Task Authenticate_issues_a_token_for_valid_credentials_only()
    {
        var service = CreateAccountService();
        await service.RegisterAsync(new RegistrationRequest(
            "login@example.com", "Passw0rd!long", null, "GB",
            "2026-08-consumer", null, null));

        var ok = await service.AuthenticateAsync("LOGIN@example.com", "Passw0rd!long");
        Assert.True(ok.Succeeded);
        Assert.False(string.IsNullOrWhiteSpace(ok.Token));

        var bad = await service.AuthenticateAsync("login@example.com", "wrong");
        Assert.False(bad.Succeeded);
    }

    [Fact]
    public async Task Non_admin_cannot_approve_channels()
    {
        var service = CreateChannelService();

        var result = await service.ApproveChannelAsync(
            "google_ads", "promotion", Guid.NewGuid(), AppUserRoles.RegularUser);

        Assert.False(result.Succeeded);
        Assert.Equal("forbidden_role", result.Violation);
        Assert.Empty(await _db.Set<ApprovedChannel>().ToListAsync());
    }

    [Fact]
    public async Task Domestic_promotion_channels_can_never_be_approved_and_are_audited()
    {
        var service = CreateChannelService();

        foreach (var prohibited in new[] { "xiaohongshu", "douyin", "wechat_domestic_groups" })
        {
            var result = await service.ApproveChannelAsync(
                prohibited, "promotion", Guid.NewGuid(), AppUserRoles.Administrator);

            Assert.False(result.Succeeded);
            Assert.Equal("channel_prohibited", result.Violation);
        }

        Assert.Empty(await _db.Set<ApprovedChannel>().ToListAsync());
        Assert.Equal(3, await _db.Set<AuditEvent>()
            .Where(e => e.Action == "approval.policy_violation")
            .CountAsync());
    }

    [Fact]
    public async Task Overseas_channel_approval_is_recorded_once()
    {
        var service = CreateChannelService();
        var adminId = Guid.NewGuid();

        var first = await service.ApproveChannelAsync(
            "Google_Ads ", "promotion", adminId, AppUserRoles.Administrator);
        var duplicate = await service.ApproveChannelAsync(
            "google_ads", "promotion", adminId, AppUserRoles.Administrator);

        Assert.True(first.Succeeded);
        Assert.Equal("already_approved", duplicate.Violation);
        Assert.Single(await service.ListApprovedAsync());
    }

    [Fact]
    public void Payment_methods_never_include_domestic_rails()
    {
        var service = CreateChannelService();

        var allowed = service.AllowedPaymentMethods();

        Assert.Contains("stripe", allowed);
        Assert.DoesNotContain(allowed, m => m is "wechat_pay" or "alipay");
    }

    public void Dispose()
    {
        _db.Dispose();
        _connection.Dispose();
    }
}

/// <summary>Deterministic clock for reproducible timestamps.</summary>
public sealed class FakeClock : TimeProvider
{
    public DateTimeOffset Now { get; private set; } = new(2026, 8, 23, 12, 0, 0, TimeSpan.Zero);

    public override DateTimeOffset GetUtcNow()
    {
        return Now;
    }

    public void Advance(TimeSpan by)
    {
        Now += by;
    }
}
