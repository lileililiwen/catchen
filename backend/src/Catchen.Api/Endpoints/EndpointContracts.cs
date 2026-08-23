namespace Catchen.Api.Endpoints;

public sealed record RegisterEndpointRequest(
    string Email,
    string Password,
    string? Phone,
    string? DeclaredCountryCode,
    string AgreementVersionAccepted);

public sealed record LoginEndpointRequest(string Email, string Password);

public sealed record ApproveChannelRequest(string Channel, string Kind);
