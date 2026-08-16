using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using MilesManager.Application.Features.Users.Abstractions;
using MilesManager.Domain.Users;
using Microsoft.Extensions.Configuration;
using MilesManager.Application.Features.Users.DTOs;

namespace MilesManager.Infrastructure.Security;

public sealed class JwtTokenService(IConfiguration configuration) : IJwtTokenService
{
    private readonly string _issuer = configuration["Jwt:Issuer"] ?? throw new InvalidOperationException("Missing JWT issuer.");
    private readonly string _audience = configuration["Jwt:Audience"] ?? throw new InvalidOperationException("Missing JWT audience.");
    private readonly string _key = configuration["Jwt:Key"] ?? throw new InvalidOperationException("Missing JWT signing key.");
    private readonly int _accessTokenMinutes = int.TryParse(configuration["Jwt:AccessTokenMinutes"], out var minutes) ? minutes : 120;

    public IssuedToken CreateToken(AppUser user)
    {
        var now = DateTimeOffset.UtcNow;
        var expires = now.AddMinutes(_accessTokenMinutes);
        var header = new Dictionary<string, object>
        {
            ["alg"] = "HS256",
            ["typ"] = "JWT"
        };

        var payload = new Dictionary<string, object>
        {
            ["sub"] = user.Id.ToString(),
            ["email"] = user.Email,
            ["name"] = user.FullName,

            ["role"] = user.UserRoles.Select(r => r.Role.Name).ToArray(),

            ["jti"] = Guid.NewGuid().ToString("N"),
            ["iat"] = now.ToUnixTimeSeconds(),
            ["nbf"] = now.ToUnixTimeSeconds(),
            ["exp"] = expires.ToUnixTimeSeconds(),
            ["iss"] = _issuer,
            ["aud"] = _audience,
            ["tv"] = user.TokenVersion
        };

        var unsignedToken = $"{Base64UrlEncode(JsonSerializer.SerializeToUtf8Bytes(header))}.{Base64UrlEncode(JsonSerializer.SerializeToUtf8Bytes(payload))}";
        var signature = Sign(unsignedToken);
        return new IssuedToken($"{unsignedToken}.{signature}", expires);
    }

    private string Sign(string unsignedToken)
    {
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_key));
        var hash = hmac.ComputeHash(Encoding.ASCII.GetBytes(unsignedToken));
        return Base64UrlEncode(hash);
    }

    private static string Base64UrlEncode(byte[] bytes)
    {
        return Convert.ToBase64String(bytes)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
    }
}
