namespace MilesManager.Application.Features.Users.Models;

public enum OperationStatus
{
    Success = 1,
    NotFound = 2,
    Conflict = 3,
    Unauthorized = 4
}

public sealed record OperationResult<T>(OperationStatus Status, T? Value = default)
{
    public static OperationResult<T> Success(T value) => new(OperationStatus.Success, value);
    public static OperationResult<T> NotFound() => new(OperationStatus.NotFound);
    public static OperationResult<T> Conflict() => new(OperationStatus.Conflict);
    public static OperationResult<T> Unauthorized() => new(OperationStatus.Unauthorized);
}

public sealed record UserSummary(Guid Id, string Email, string FullName, bool IsActive, DateTimeOffset CreatedAtUtc);

public sealed record UserDetails(Guid Id, string Email, string FullName, bool IsActive, DateTimeOffset CreatedAtUtc, DateTimeOffset? UpdatedAtUtc, DateTimeOffset? LastLoginAtUtc);

public sealed record IssuedToken(string AccessToken, DateTimeOffset ExpiresAtUtc);

public sealed record AuthSession(IssuedToken Token, UserSummary User);
