using Catchen.Catalog.Models;
using Catchen.Catalog.Services;
using Catchen.Commerce.Models;
using Catchen.Editorial.Models;
using Catchen.Identity.Models;

namespace Catchen.Api.Endpoints;

public sealed record RegisterEndpointRequest(
    string Email,
    string Password,
    string? Phone,
    string? DeclaredCountryCode,
    string AgreementVersionAccepted);

public sealed record LoginEndpointRequest(string Email, string Password);

public sealed record ApproveChannelRequest(string Channel, string Kind);

public sealed record RegisterResponse(Guid UserId);

public sealed record LoginResponse(string Token, DateTimeOffset ExpiresAtUtc);

public sealed record PaymentMethodsResponse(IReadOnlyList<string> Allowed);

public sealed record ApprovalResponse(Guid Id);

public sealed record ApprovedChannelsResponse(IReadOnlyList<ApprovedChannel> Approvals);

public sealed record CreateDraftRequest(
    RecipeContent Content,
    ProvenanceEvidence Provenance,
    bool IsFree);

public sealed record CommentRequest(string Text);

public sealed record ReasonCodeRequest(string ReasonCode);

public sealed record ShoppingListRequest(IReadOnlyList<Guid> RecipeIds);

public sealed record CatalogListResponse(IReadOnlyList<CatalogSummary> Items);

public sealed record CommentListResponse(IReadOnlyList<RecipeComment> Comments);

public sealed record CommentCreatedResponse(Guid Id);

public sealed record WorkflowCreatedResponse(Guid? DraftId, Guid? PublishedRecipeId);

public sealed record CheckoutCreatedResponse(Guid OrderId, string? CheckoutUrl);

public sealed record OrderListResponse(IReadOnlyList<Order> Orders);
