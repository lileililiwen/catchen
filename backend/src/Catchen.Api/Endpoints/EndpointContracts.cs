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
