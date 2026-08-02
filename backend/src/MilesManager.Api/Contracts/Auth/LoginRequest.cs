namespace MilesManager.Api.Contracts.Auth;

/// <summary>
/// Represents a request to log in a user with their email and password.
/// </summary>
/// <param name="Email"></param>
/// <param name="Password"></param>
public sealed record LoginRequest(string Email, string Password);
