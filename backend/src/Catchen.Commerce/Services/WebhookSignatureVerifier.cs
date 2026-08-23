using System.Security.Cryptography;
using System.Text;

namespace Catchen.Commerce.Services;

/// <summary>
/// Verifies provider webhook signatures. Implements the Stripe signing
/// scheme (also used by several compatible providers): a
/// "t=timestamp,v1=hmac" header where v1 is HMAC-SHA256(secret,
/// "{timestamp}.{body}"), with replay protection via a tolerance window.
/// </summary>
public interface IWebhookSignatureVerifier
{
    bool Verify(string secret, string body, string signatureHeader, DateTimeOffset now);
}

public sealed class StripeSchemeSignatureVerifier : IWebhookSignatureVerifier
{
    private static readonly TimeSpan _tolerance = TimeSpan.FromMinutes(5);

    public bool Verify(string secret, string body, string signatureHeader, DateTimeOffset now)
    {
        if (string.IsNullOrWhiteSpace(secret) || string.IsNullOrWhiteSpace(body)
            || string.IsNullOrWhiteSpace(signatureHeader))
        {
            return false;
        }

        string? timestamp = null;
        var signatures = new List<string>();

        foreach (var part in signatureHeader.Split(','))
        {
            var kv = part.Trim().Split('=', 2);
            if (kv.Length != 2)
            {
                continue;
            }

            if (kv[0] == "t")
            {
                timestamp = kv[1];
            }
            else if (kv[0] == "v1")
            {
                signatures.Add(kv[1]);
            }
        }

        if (timestamp is null || signatures.Count == 0
            || !long.TryParse(timestamp, out var unixSeconds))
        {
            return false;
        }

        var signedAt = DateTimeOffset.FromUnixTimeSeconds(unixSeconds);
        if (Math.Abs((now - signedAt).TotalMinutes) > _tolerance.TotalMinutes)
        {
            return false; // replay outside the tolerance window
        }

        var expected = Convert.ToHexString(
            HMACSHA256.HashData(
                Encoding.UTF8.GetBytes(secret),
                Encoding.UTF8.GetBytes($"{timestamp}.{body}"))).ToLowerInvariant();

        return signatures.Any(provided =>
            CryptographicOperations.FixedTimeEquals(
                Encoding.UTF8.GetBytes(expected),
                Encoding.UTF8.GetBytes(provided.ToLowerInvariant())));
    }
}
