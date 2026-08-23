using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Catchen.Identity.Models;
using Microsoft.EntityFrameworkCore;

namespace Catchen.Identity.Services;

public interface IAuditWriter
{
    Task WriteAsync(
        string category,
        string action,
        Guid? actorUserId,
        string subjectType,
        string subjectId,
        object payload,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Append-only audit writer. Payloads are serialized as-is; callers MUST pass
/// reason codes and digests only — never raw personal data or secrets.
/// </summary>
public sealed class AuditWriter : IAuditWriter
{
    private static readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web);
    private readonly DbContext _db;
    private readonly TimeProvider _clock;

    public AuditWriter(DbContext db, TimeProvider clock)
    {
        _db = db;
        _clock = clock;
    }

    public async Task WriteAsync(
        string category,
        string action,
        Guid? actorUserId,
        string subjectType,
        string subjectId,
        object payload,
        CancellationToken cancellationToken = default)
    {
        _db.Set<AuditEvent>().Add(new AuditEvent
        {
            OccurredAtUtc = _clock.GetUtcNow(),
            Category = category,
            Action = action,
            ActorUserId = actorUserId,
            SubjectType = subjectType,
            SubjectId = subjectId,
            PayloadJson = JsonSerializer.Serialize(payload, _jsonOptions),
        });

        await _db.SaveChangesAsync(cancellationToken);
    }
}

public static class AuditEvidence
{
    public static string HashIp(string? clientIp)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(clientIp ?? "unknown"));
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }

    public static string TruncateUserAgent(string? userAgent)
    {
        return string.IsNullOrWhiteSpace(userAgent) ? "unknown" : userAgent.Trim()[..Math.Min(128, userAgent.Trim().Length)];
    }
}
