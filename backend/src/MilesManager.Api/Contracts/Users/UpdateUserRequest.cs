namespace MilesManager.Api.Contracts.Users;

/// <summary>
/// Represents a request to update an existing user's information, including their email, full name, password (optional), and active status.
/// </summary>
/// <param name="Email"></param>
/// <param name="FullName"></param>
/// <param name="Password"></param>
/// <param name="IsActive"></param>
public sealed record UpdateUserRequest(string Email, string FullName, string? Password, bool IsActive);
