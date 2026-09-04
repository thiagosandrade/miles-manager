using MilesManager.Application.Features.UserRoles.DTOs;

namespace MilesManager.Application.Features.Users.DTOs;

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

public sealed class UserDetailsDTO
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTimeOffset CreatedAtUtc { get; set; }
    public DateTimeOffset? UpdatedAtUtc { get; set; }
    public DateTimeOffset? LastLoginAtUtc { get; set; }

    public ICollection<UserRoleDTO>? Roles { get; set; } = [];
}

public sealed record IssuedToken(string AccessToken, DateTimeOffset ExpiresAtUtc);

public sealed record AuthSession(IssuedToken Token);
