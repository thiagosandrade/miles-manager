using MediatR;
using MilesManager.Api.Contracts.Auth;
using MilesManager.Application.Features.Users.Commands.Login;
using MilesManager.Application.Features.Users.Commands.Logout;
using MilesManager.Application.Features.Users.Models;
using System.Security.Claims;

namespace MilesManager.Api.Features.Auth;

/// <summary>
/// Provides extension methods for mapping authentication endpoints in the API.
/// </summary>
public static class AuthEndpoints
{
    /// <summary>
    /// Maps authentication endpoints.
    /// </summary>
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/auth/login", async (LoginRequest request, IMediator mediator, CancellationToken cancellationToken) =>
            {
                var result = await mediator.Send(new LoginCommand(request.Email, request.Password), cancellationToken);
                return result.Status switch
                {
                    OperationStatus.Success => Results.Ok(result.Value),
                    OperationStatus.Unauthorized => Results.Unauthorized(),
                    _ => Results.BadRequest()
                };
            })
            .AllowAnonymous()
            .WithTags("Auth")
            .WithName("Login")
            .WithSummary("Authenticates a user and returns a JWT.");

        app.MapPost("/api/auth/logout", async (ClaimsPrincipal principal, IMediator mediator, CancellationToken cancellationToken) =>
            {
                var subject = principal.FindFirstValue("sub");
                if (!Guid.TryParse(subject, out var userId))
                {
                    return Results.Unauthorized();
                }

                var result = await mediator.Send(new LogoutCommand(userId), cancellationToken);
                return result == OperationStatus.Success ? Results.NoContent() : Results.NotFound();
            })
            .RequireAuthorization()
            .WithTags("Auth")
            .WithName("Logout")
            .WithSummary("Invalidates the current JWT by rotating the token version.");

        return app;
    }
}
