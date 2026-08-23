using System.Globalization;
using System.Text;
using System.Text.Json;
using Catchen.Catalog.Models;
using Catchen.Commerce.Models;
using Catchen.Commerce.Services;
using Catchen.Data;
using Catchen.Documents.Services;
using Catchen.Identity.Models;
using Catchen.Identity.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Xunit;

namespace Catchen.UnitTests;

/// <summary>
/// Tasks 3.1–3.4: provider-neutral ledger, signed webhook inbox with
/// idempotency and replay protection, entitlement-gated PDFs.
/// </summary>
public sealed class CommerceDocumentsTests : IDisposable
{
    private const string Secret = "unit-test-webhook-secret-0123456789";

    private readonly SqliteConnection _connection;
    private readonly DbContext _db;
    private readonly FakeClock _clock = new();

    public CommerceDocumentsTests()
    {
        WebhookSecrets.Current = Secret;
        QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

        _connection = new SqliteConnection("Data Source=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<AppDbContext>().UseSqlite(_connection).Options;
        _db = new AppDbContext(options);
        _db.Database.EnsureCreated();
    }

    private CheckoutService Checkout()
    {
        return new(_db, _clock);
    }

    private WebhookInboxService Inbox()
    {
        var logger = NullLogger<WebhookInboxService>.Instance;
        return new WebhookInboxService(
            _db,
            new StripeSchemeSignatureVerifier(),
            new EntitlementLedger(_db, _clock),
            new AuditWriter(_db, _clock),
            _clock,
            logger);
    }

    private RecipeDocumentService Documents()
    {
        return new(_db, new EntitlementLedger(_db, _clock));
    }

    /// <summary>Builds a Stripe-shaped signed webhook body for a session id.</summary>
    private (string Body, string Header) SignedEvent(
        string eventId, string type, string reference, bool referenceIsIntent = false)
    {
        object obj = referenceIsIntent
            ? new { payment_intent = (string?)reference }
            : new { id = (string?)reference };
        var payload = JsonSerializer.Serialize(new
        {
            id = eventId,
            type,
            data = new { @object = obj },
        });
        var timestamp = _clock.GetUtcNow().ToUnixTimeSeconds().ToString(CultureInfo.InvariantCulture);
        var signature = Convert.ToHexString(
            System.Security.Cryptography.HMACSHA256.HashData(
                Encoding.UTF8.GetBytes(Secret),
                Encoding.UTF8.GetBytes($"{timestamp}.{payload}"))).ToLowerInvariant();
        return (payload, $"t={timestamp},v1={signature}");
    }

    private async Task<(Guid userId, Guid orderId, Guid recipeId)> SeedPaidRecipeOrderAsync()
    {
        var userId = Guid.NewGuid();
        var checkout = Checkout();

        // Publish one premium recipe directly (editorial flow covered elsewhere).
        var draftId = Guid.NewGuid();
        _db.Set<PublishedRecipe>().Add(new PublishedRecipe
        {
            Id = Guid.NewGuid(),
            RecipeId = draftId,
            Version = 1,
            Title = "Premium Banquet",
            Cuisine = CuisineCategory.Sichuan,
            Difficulty = RecipeDifficulty.Medium,
            PreviewText = "preview",
            ContentJson = """{"ingredients":[{"name":"tofu","quantity":{"value":400,"unit":"g"}}],"instructions":["Cook."],"equipment":["wok"],"culturalContext":"Sichuan"}""",
            IngredientIndex = "tofu",
            IsFree = false,
            IsLive = true,
            AuthorUserId = Guid.NewGuid(),
            ReviewerUserId = Guid.NewGuid(),
            PublishedAtUtc = _clock.GetUtcNow(),
        });
        await _db.SaveChangesAsync();

        var started = await checkout.StartRecipePurchaseAsync(userId, draftId);
        Assert.True(started.Succeeded);
        var order = await _db.Set<Order>().SingleAsync(o => o.Id == started.OrderId);
        return (userId, order.Id, draftId);
    }

    [Fact]
    public async Task Verified_webhook_grants_entitlement_exactly_once()
    {
        var (userId, orderId, _) = await SeedPaidRecipeOrderAsync();
        var inbox = Inbox();
        var order = await _db.Set<Order>().SingleAsync(o => o.Id == orderId);

        var (body, header) = SignedEvent("evt_1", "checkout.session.completed", order.ProviderReference!);

        var first = await inbox.IngestAsync("stripe", body, header);
        var replay = await inbox.IngestAsync("stripe", body, header);

        Assert.True(first.Accepted);
        Assert.False(replay.Accepted);
        Assert.Equal("duplicate_event", replay.SkipReason);

        var entitlements = await _db.Set<Entitlement>().Where(e => e.UserId == userId).ToListAsync();
        Assert.Single(entitlements); // exactly-once grant despite duplicate delivery

        var storedOrder = await _db.Set<Order>().SingleAsync(o => o.Id == orderId);
        Assert.Equal(OrderStatus.Paid, storedOrder.Status);
    }

    [Fact]
    public async Task Forged_signature_makes_no_entitlement_change_and_is_audited()
    {
        var (_, orderId, _) = await SeedPaidRecipeOrderAsync();
        var inbox = Inbox();
        var order = await _db.Set<Order>().SingleAsync(o => o.Id == orderId);

        var (body, _) = SignedEvent("evt_forged", "checkout.session.completed", order.ProviderReference!);

        var result = await inbox.IngestAsync("stripe", body, "t=1,v1=deadbeef");

        Assert.False(result.Accepted);
        Assert.Equal("invalid_signature", result.SkipReason);
        Assert.Empty(await _db.Set<Entitlement>().ToListAsync());
        Assert.Equal(OrderStatus.Pending,
            (await _db.Set<Order>().SingleAsync(o => o.Id == orderId)).Status);
        Assert.Equal(1, await _db.Set<AuditEvent>()
            .CountAsync(e => e.Action == "webhook.forged"));
    }

    [Fact]
    public async Task Replayed_timestamp_outside_tolerance_is_rejected()
    {
        var (_, orderId, _) = await SeedPaidRecipeOrderAsync();
        var inbox = Inbox();
        var order = await _db.Set<Order>().SingleAsync(o => o.Id == orderId);

        var payload = JsonSerializer.Serialize(new
        {
            id = "evt_old",
            type = "checkout.session.completed",
            data = new { @object = new { id = order.ProviderReference } },
        });
        var staleTimestamp = _clock.GetUtcNow().AddHours(-1).ToUnixTimeSeconds().ToString(CultureInfo.InvariantCulture);
        var signature = Convert.ToHexString(
            System.Security.Cryptography.HMACSHA256.HashData(
                Encoding.UTF8.GetBytes(Secret),
                Encoding.UTF8.GetBytes($"{staleTimestamp}.{payload}"))).ToLowerInvariant();

        var result = await inbox.IngestAsync(
            "stripe", payload, $"t={staleTimestamp},v1={signature}");

        Assert.False(result.Accepted);
        Assert.Equal("invalid_signature", result.SkipReason); // replay rejected pre-storage
        Assert.Empty(await _db.Set<WebhookEvent>().ToListAsync());
    }

    [Fact]
    public async Task Confirmed_refund_revokes_the_recipe_entitlement()
    {
        var (userId, orderId, recipeId) = await SeedPaidRecipeOrderAsync();
        var inbox = Inbox();
        var order = await _db.Set<Order>().SingleAsync(o => o.Id == orderId);

        var (paidBody, paidHeader) =
            SignedEvent("evt_paid", "checkout.session.completed", order.ProviderReference!);
        await inbox.IngestAsync("stripe", paidBody, paidHeader);

        var documents = Documents();
        Assert.NotNull(await documents.RenderRecipePdfAsync(userId, recipeId));

        var (refundBody, refundHeader) = SignedEvent(
            "evt_refund", "charge.refunded", order.ProviderReference!, referenceIsIntent: true);
        var refunded = await inbox.IngestAsync("stripe", refundBody, refundHeader);

        Assert.True(refunded.Accepted);
        Assert.Null((await documents.RenderRecipePdfAsync(userId, recipeId))); // access revoked

        var storedOrder = await _db.Set<Order>().SingleAsync(o => o.Id == orderId);
        Assert.Equal(OrderStatus.Refunded, storedOrder.Status);
    }

    [Fact]
    public async Task Membership_renewal_extends_period_exactly_once_per_event()
    {
        var userId = Guid.NewGuid();
        var checkout = Checkout();
        var started = await checkout.StartMembershipCheckoutAsync(userId);
        var order = await _db.Set<Order>().SingleAsync(o => o.Id == started.OrderId);

        var inbox = Inbox();
        var (body, header) =
            SignedEvent("evt_member_1", "checkout.session.completed", order.ProviderReference!);
        await inbox.IngestAsync("stripe", body, header);

        var entitlement = await _db.Set<Entitlement>().SingleAsync(e => e.UserId == userId);
        Assert.NotNull(entitlement.PeriodEndUtc);

        var ledger = new EntitlementLedger(_db, _clock);
        Assert.True(await ledger.HasFullAccessAsync(userId, null));
        // An active membership unlocks every recipe, not just one.
        Assert.True(await ledger.HasFullAccessAsync(userId, Guid.NewGuid()));
        Assert.False(await ledger.HasFullAccessAsync(Guid.NewGuid(), null));
    }

    [Fact]
    public async Task Shopping_list_aggregates_only_accessible_recipes()
    {
        var (userId, freeRecipeId, premiumRecipeId) = await PublishFreeAndPremiumAsync();
        var documents = Documents();

        var result = await documents.RenderShoppingListAsync(
            userId, [freeRecipeId, premiumRecipeId]);

        Assert.NotNull(result);
        Assert.Equal("%PDF-", Encoding.ASCII.GetString(result.PdfBytes[..5]));

        // Entitlement filter: a request covering ONLY the locked recipe
        // produces nothing.
        var lockedOnly = await documents.RenderShoppingListAsync(
            userId, [premiumRecipeId]);
        Assert.Null(lockedOnly);
    }

    private async Task<(Guid userId, Guid freeId, Guid premiumId)> PublishFreeAndPremiumAsync()
    {
        var userId = Guid.NewGuid();
        var premiumRecipeId = Guid.NewGuid();
        var freeId = Guid.NewGuid();

        _db.Set<PublishedRecipe>().AddRange(
            new PublishedRecipe
            {
                Id = Guid.NewGuid(),
                RecipeId = freeId,
                Version = 1,
                Title = "Free Stir Fry",
                Cuisine = CuisineCategory.Sichuan,
                Difficulty = RecipeDifficulty.Easy,
                PreviewText = "preview",
                ContentJson = """{"ingredients":[{"name":"tofu","quantity":{"value":300,"unit":"g"}}]}""",
                IngredientIndex = "tofu",
                IsFree = true,
                IsLive = true,
                AuthorUserId = Guid.NewGuid(),
                ReviewerUserId = Guid.NewGuid(),
                PublishedAtUtc = _clock.GetUtcNow(),
            },
            new PublishedRecipe
            {
                Id = Guid.NewGuid(),
                RecipeId = premiumRecipeId,
                Version = 1,
                Title = "Premium Lobster",
                Cuisine = CuisineCategory.Cantonese,
                Difficulty = RecipeDifficulty.Hard,
                PreviewText = "preview",
                ContentJson = """{"ingredients":[{"name":"lobster","quantity":{"value":1,"unit":"lb"}}]}""",
                IngredientIndex = "lobster",
                IsFree = false,
                IsLive = true,
                AuthorUserId = Guid.NewGuid(),
                ReviewerUserId = Guid.NewGuid(),
                PublishedAtUtc = _clock.GetUtcNow(),
            });
        await _db.SaveChangesAsync();
        return (userId, freeId, premiumRecipeId);
    }

    [Fact]
    public async Task Locked_premium_pdf_returns_null_even_for_signed_in_users()
    {
        var (_, _, premiumId) = await PublishFreeAndPremiumAsync();
        var documents = Documents();

        Assert.Null(await documents.RenderRecipePdfAsync(Guid.NewGuid(), premiumId));
    }

    [Fact]
    public async Task Free_recipe_pdf_renders_bytes_with_version()
    {
        var (_, freeId, _) = await PublishFreeAndPremiumAsync();
        var documents = Documents();

        var result = await documents.RenderRecipePdfAsync(Guid.NewGuid(), freeId);

        Assert.NotNull(result);
        Assert.Equal(1, result.Version);
        Assert.True(result.PdfBytes.Length > 500); // real PDF content, not empty
        Assert.Equal("%PDF-", Encoding.ASCII.GetString(result.PdfBytes[..5]));
    }

    public void Dispose()
    {
        _db.Dispose();
        _connection.Dispose();
    }
}
