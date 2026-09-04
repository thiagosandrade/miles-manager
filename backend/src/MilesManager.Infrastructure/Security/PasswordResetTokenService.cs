using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using MilesManager.Application.Abstractions;
using MilesManager.Domain.Users;

namespace MilesManager.Infrastructure.Security;

public sealed class PasswordResetTokenService(IConfiguration configuration) : IPasswordResetTokenService
{
    private readonly string _issuer = configuration["Jwt:Issuer"] ?? throw new InvalidOperationException("Missing JWT issuer.");
    private readonly string _audience = configuration["Jwt:Audience"] ?? throw new InvalidOperationException("Missing JWT audience.");
    private readonly string _key = configuration["Jwt:Key"] ?? throw new InvalidOperationException("Missing JWT signing key.");

    public string CreateToken(AppUser user, DateTimeOffset expiresAt)
    {
        var header = new Dictionary<string, object>
        {
            ["alg"] = "HS256",
            ["typ"] = "JWT"
        };

        var payload = new Dictionary<string, object>
        {
            ["sub"] = user.Id.ToString(),
            ["purpose"] = "password-reset",
            ["tv"] = user.TokenVersion,
            ["iat"] = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            ["exp"] = expiresAt.ToUnixTimeSeconds(),
            ["iss"] = _issuer,
            ["aud"] = _audience
        };

        var unsignedToken = $"{Encode(JsonSerializer.SerializeToUtf8Bytes(header))}.{Encode(JsonSerializer.SerializeToUtf8Bytes(payload))}";
        return $"{unsignedToken}.{Sign(unsignedToken)}";
    }

    public bool TryValidateToken(string token, out PasswordResetTokenData data)
    {
        data = default!;

        try
        {
            var parts = token.Split('.');
            if (parts.Length != 3)
            {
                return false;
            }

            var unsignedToken = $"{parts[0]}.{parts[1]}";
            var expectedSignature = Sign(unsignedToken);
            var actualSignature = Decode(parts[2]);
            var expectedSignatureBytes = Decode(expectedSignature);

            if (!CryptographicOperations.FixedTimeEquals(actualSignature, expectedSignatureBytes))
            {
                return false;
            }

            using var payloadDocument = JsonDocument.Parse(Decode(parts[1]));
            var payload = payloadDocument.RootElement;

            if (!string.Equals(payload.GetProperty("purpose").GetString(), "password-reset", StringComparison.Ordinal))
            {
                return false;
            }

            if (!string.Equals(payload.GetProperty("iss").GetString(), _issuer, StringComparison.Ordinal) ||
                !string.Equals(payload.GetProperty("aud").GetString(), _audience, StringComparison.Ordinal))
            {
                return false;
            }

            var userId = Guid.Parse(payload.GetProperty("sub").GetString()!);
            var tokenVersion = payload.GetProperty("tv").GetInt32();
            var expiresAt = DateTimeOffset.FromUnixTimeSeconds(payload.GetProperty("exp").GetInt64());

            if (expiresAt <= DateTimeOffset.UtcNow)
            {
                return false;
            }

            data = new PasswordResetTokenData(userId, tokenVersion, expiresAt);
            return true;
        }
        catch (Exception) when (Exception is FormatException or JsonException or KeyNotFoundException)
        {
            return false;
        }
    }

    private string Sign(string unsignedToken)
    {
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_key));
        return Encode(hmac.ComputeHash(Encoding.ASCII.GetBytes(unsignedToken)));
    }

    private static string Encode(byte[] bytes) => Convert.ToBase64String(bytes)
        .TrimEnd('=')
        .Replace('+', '-')
        .Replace('/', '_');

    private static byte[] Decode(string value)
    {
        var padded = value.Replace('-', '+').Replace('_', '/');
        padded += new string('=', (4 - padded.Length % 4) % 4);
        return Convert.FromBase64String(padded);
    }
}
