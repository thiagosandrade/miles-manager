#nullable enable
namespace MilesManager.Application.Common;

public enum ResultStatus
{
    Success = 1,
    NotFound = 2,
    ValidationFailed = 3,
    Conflict = 4,
    Unauthorized = 5
}

public sealed record Result<T>(ResultStatus Status, T? Value = default, string? Error = null)
{
    public static Result<T> Success(T value) => new(ResultStatus.Success, value);
    public static Result<T> NotFound() => new(ResultStatus.NotFound);
    public static Result<T> ValidationFailed(string error) => new(ResultStatus.ValidationFailed, default, error);
    public static Result<T> Conflict(string? error = null) => new(ResultStatus.Conflict, default, error);
    public static Result<T> Unauthorized() => new(ResultStatus.Unauthorized);
}

public sealed record Result(ResultStatus Status, string? Error = null)
{
    public static Result Success() => new(ResultStatus.Success);
    public static Result NotFound() => new(ResultStatus.NotFound);
    public static Result ValidationFailed(string error) => new(ResultStatus.ValidationFailed, error);
    public static Result Conflict(string? error = null) => new(ResultStatus.Conflict, error);
    public static Result Unauthorized() => new(ResultStatus.Unauthorized);
}
