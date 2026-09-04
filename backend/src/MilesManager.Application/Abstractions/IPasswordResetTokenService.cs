using MilesManager.Domain.Users;

namespace MilesManager.Application.Abstractions;

public interface IPasswordResetTokenService
{
    string CreateToken(AppUser user, DateTimeOffset expiresAt);
    bool TryValidateToken(string token, out PasswordResetTokenData data);
}

public sealed record PasswordResetTokenData(Guid UserId, int TokenVersion, DateTimeOffset ExpiresAtUtc);
