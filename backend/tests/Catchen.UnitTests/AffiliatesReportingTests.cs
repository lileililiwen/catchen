using Catchen.Affiliates.Models;
using Catchen.Affiliates.Services;
using Catchen.Catalog.Models;
using Catchen.Catalog.Services;
using Catchen.Commerce.Models;
using Catchen.Commerce.Services;
using Catchen.Data;
using Catchen.Identity.Models;
using Catchen.Identity.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Catchen.UnitTests;

/// <summary>
/// Tasks 3.4 + 4.1 + 4.2: reconciled order reporting, allowlisted affiliate
/// redirects with privacy-minimized attribution, commission statement
/// import with deduplication, and ops moderation/publication counters.
/// </summary>
public sealed class AffiliatesReportingTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly DbContext _db;
    private readonly FakeClock _clock = new();

    public AffiliatesReportingTests()
    {
        _connection = new SqliteConnection("Data Source=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<AppDbContext>().UseSqlite(_connection).Options;
        _db = new AppDbContext(options);
        _db.Database.EnsureCreated();
    }

    private AffiliateLinkService Links()
    {
        return new(_db, new AuditWriter(_db, _clock), _clock);
    }

    private CommissionImportService Import()
    {
        return new(_db, _clock);
    }

    [Fact]
    public async Task Admin_registers_allowlisted_merchant_and_click_redirects_with_tag()
    {
        var links = Links();
        var adminId = Guid.NewGuid();

        var registered = await links.RegisterMerchantAsync(
            "amazon_kitchen", "Amazon Kitchen",
            "https://www.amazon.com/dp/example", "catchen-21",
            adminId, AppUserRoles.Administrator);
        Assert.True(registered.Allowed);

        var resolved = await links.ResolveRedirectAsync(
            "AMAZON_KITCHEN", "summer-campaign", Guid.NewGuid());

        Assert.True(resolved.Allowed);
        Assert.StartsWith("https://www.amazon.com/dp/example?", resolved.Destination);
        Assert.Contains("tag=catchen-21", resolved.Destination);
        Assert.Contains("cid=summer-campaign", resolved.Destination);

        // Privacy-minimized click record: merchant + pseudonym only.
        var click = await _db.Set<AffiliateClick>().SingleAsync();
        Assert.Equal("amazon_kitchen", click.MerchantSlug);
        Assert.Equal(64, click.VisitorPseudonym.Length);
    }

    [Fact]
    public async Task Non_admin_cannot_register_merchants()
    {
        var result = await Links().RegisterMerchantAsync(
            "amazon_kitchen", "Amazon Kitchen",
            "https://www.amazon.com/x", "tag",
            Guid.NewGuid(), AppUserRoles.RegularUser);

        Assert.False(result.Allowed);
        Assert.Equal("forbidden_role", result.Violation);
    }

    [Fact]
    public async Task Domestic_merchant_slugs_are_prohibited_and_audited()
    {
        foreach (var prohibited in new[] { "xiaohongshu", "douyin", "taobao" })
        {
            var result = await Links().RegisterMerchantAsync(
                prohibited, prohibited, "https://example.com/x", "tag",
                Guid.NewGuid(), AppUserRoles.Administrator);

            Assert.False(result.Allowed);
            Assert.Equal("merchant_prohibited", result.Violation);
        }

        Assert.Empty(await _db.Set<AffiliateMerchant>().ToListAsync());
        Assert.Equal(3, await _db.Set<AuditEvent>()
            .CountAsync(e => e.Action == "merchant.policy_violation"));
    }

    [Fact]
    public async Task Non_https_base_urls_are_rejected()
    {
        var result = await Links().RegisterMerchantAsync(
            "insecure", "Insecure", "http://www.example.com", "tag",
            Guid.NewGuid(), AppUserRoles.Administrator);

        Assert.False(result.Allowed);
        Assert.Equal("merchant_invalid", result.Violation);
    }

    [Fact]
    public async Task Redirect_to_unregistered_slug_is_blocked()
    {
        var result = await Links().ResolveRedirectAsync("unknown_shop", null, null);

        Assert.False(result.Allowed);
        Assert.Equal("merchant_not_allowlisted", result.Violation);
    }

    [Fact]
    public async Task Commission_import_deduplicates_validates_and_reports()
    {
        var links = Links();
        await links.RegisterMerchantAsync(
            "amazon_kitchen", "Amazon Kitchen", "https://www.amazon.com/x", "tag",
            Guid.NewGuid(), AppUserRoles.Administrator);

        var import = Import();
        var rows = new List<CommissionImportRow>
        {
            new("row-1", "amazon_kitchen", 1250, "USD"),
            new("row-1", "amazon_kitchen", 1250, "USD"), // duplicate
            new("row-2", "unknown_shop", 500, "USD"),    // unknown merchant
            new("row-3", "amazon_kitchen", -1, "USD"),   // invalid amount
            new("row-4", "amazon_kitchen", 100, "CNY"),  // unsupported currency
        };

        var report = await import.ImportAsync("amazon", rows);

        Assert.Equal(1, report.Accepted);
        Assert.Equal(1, report.Duplicates);
        Assert.Equal(3, report.Rejected);

        // Re-import: everything duplicates now.
        var second = await import.ImportAsync("amazon", rows.Take(1).ToList());
        Assert.Equal(0, second.Accepted);
        Assert.Equal(1, second.Duplicates);

        var acceptedRows = await import.AcceptedRowsAsync();
        Assert.Single(acceptedRows);
        Assert.Equal(1250, acceptedRows[0].AmountMinorUnits);
    }

    [Fact]
    public async Task Order_report_filters_by_status_and_totals_paid_usd()
    {
        var userId = Guid.NewGuid();
        var checkout = new CheckoutService(_db, _clock);

        // Seed a live premium recipe so the single-recipe order is creatable.
        var premiumRecipeId = Guid.NewGuid();
        _db.Set<PublishedRecipe>().Add(new PublishedRecipe
        {
            Id = Guid.NewGuid(),
            RecipeId = premiumRecipeId,
            Version = 1,
            Title = "P",
            Cuisine = CuisineCategory.Sichuan,
            Difficulty = RecipeDifficulty.Medium,
            PreviewText = "p",
            ContentJson = "{}",
            IngredientIndex = "",
            IsFree = false,
            IsLive = true,
            AuthorUserId = Guid.NewGuid(),
            ReviewerUserId = Guid.NewGuid(),
            PublishedAtUtc = _clock.GetUtcNow(),
        });
        await _db.SaveChangesAsync();

        var membership = await checkout.StartMembershipCheckoutAsync(userId);
        var recipeOrder = await checkout.StartRecipePurchaseAsync(userId, premiumRecipeId);

        // Pay the membership order via ledger event.
        var membershipRow = await _db.Set<Order>().SingleAsync(o => o.Id == membership.OrderId!.Value);
        membershipRow.Status = OrderStatus.Paid;
        membershipRow.PaidAtUtc = _clock.GetUtcNow();
        await _db.SaveChangesAsync();

        var reporting = new OrderReportingService(_db);

        var all = await reporting.ReportAsync(null, null, null, null, null);
        Assert.Equal(2, all.TotalOrders);
        Assert.Equal(membershipRow.AmountMinorUnits, all.PaidAmountMinorUnits);

        var paidOnly = await reporting.ReportAsync(
            null, null, null, null, OrderStatus.Paid);
        Assert.Single(paidOnly.Rows);

        var pendingOnly = await reporting.ReportAsync(
            null, null, null, null, OrderStatus.Pending);
        Assert.Single(pendingOnly.Rows);
        Assert.Equal(recipeOrder.OrderId, pendingOnly.Rows[0].OrderId);
    }

    [Fact]
    public async Task Moderation_counters_reflect_visible_and_hidden_comments()
    {
        var comments = new CommentsService(_db, _clock);
        var recipeId = Guid.NewGuid();
        var userA = Guid.NewGuid();
        var userB = Guid.NewGuid();

        _db.Set<PublishedRecipe>().Add(new PublishedRecipe
        {
            Id = Guid.NewGuid(),
            RecipeId = recipeId,
            Version = 1,
            Title = "T",
            Cuisine = CuisineCategory.Sichuan,
            Difficulty = RecipeDifficulty.Easy,
            PreviewText = "p",
            ContentJson = "{}",
            IngredientIndex = "",
            IsFree = true,
            IsLive = true,
            AuthorUserId = Guid.NewGuid(),
            ReviewerUserId = Guid.NewGuid(),
            PublishedAtUtc = _clock.GetUtcNow(),
        });
        await _db.SaveChangesAsync();

        await comments.AddAsync(userA, recipeId, "visible one");
        var hidden = await comments.AddAsync(userB, recipeId, "to hide");
        await comments.HideAsync(hidden.CommentId!.Value, "off_topic");

        var (visible, hiddenCount) = await comments.CountsAsync();
        Assert.Equal(1, visible);
        Assert.Equal(1, hiddenCount);
    }

    public void Dispose()
    {
        _db.Dispose();
        _connection.Dispose();
    }
}
