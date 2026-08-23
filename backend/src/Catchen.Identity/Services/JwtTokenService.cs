using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Catchen.Identity.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Catchen.Identity.Services;

public interface IJwtTokenService
{
    (string Token, DateTimeOffset ExpiresAtUtc) Issue(AppUser user);
}

public sealed class JwtTokenService : IJwtTokenService
{
    private readonly IdentityOptions _options;
    private readonly TimeProvider _clock;

    public JwtTokenService(IOptions<IdentityOptions> options, TimeProvider clock)
    {
        _options = options.Value;
        _clock = clock;
    }

    public (string Token, DateTimeOffset ExpiresAtUtc) Issue(AppUser user)
    {
        var now = _clock.GetUtcNow();
        var expires = now.AddMinutes(_options.JwtLifetimeMinutes);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(ClaimTypes.Role, user.Role),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.JwtSecret)),
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _options.JwtIssuer,
            audience: _options.JwtAudience,
            claims: claims,
            notBefore: now.UtcDateTime,
            expires: expires.UtcDateTime,
            signingCredentials: credentials);

        return (new JwtSecurityTokenHandler().WriteToken(token), expires);
    }
}
