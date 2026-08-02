namespace MilesManager.Api.Contracts.Users;

/// <summary>
/// Represents a request to create a new user with their email, full name, and password.
/// </summary>
/// <param name="Email"></param>
/// <param name="FullName"></param>
/// <param name="Password"></param>
public sealed record CreateUserRequest(string Email, string FullName, string Password);
