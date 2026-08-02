using MediatR;
using MilesManager.Api.Contracts.Users;
using MilesManager.Application.Features.Users.Commands.CreateUser;
using MilesManager.Application.Features.Users.Commands.DeleteUser;
using MilesManager.Application.Features.Users.Commands.UpdateUser;
using MilesManager.Application.Features.Users.Models;
using MilesManager.Application.Features.Users.Queries.GetUserById;
using MilesManager.Application.Features.Users.Queries.GetUsers;

namespace MilesManager.Api.Features.Users;

/// <summary>
/// Provides extension methods for mapping user management endpoints in the API.
/// </summary>
public static class UserEndpoints
{
    /// <summary>
    /// Maps user management endpoints.
    /// </summary>
    public static IEndpointRouteBuilder MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/users", async (CreateUserRequest request, IMediator mediator, CancellationToken cancellationToken) =>
            {
                var result = await mediator.Send(new CreateUserCommand(request.Email, request.FullName, request.Password), cancellationToken);
                return result.Status switch
                {
                    OperationStatus.Success => Results.Created($"/api/users/{result.Value!.Id}", result.Value),
                    OperationStatus.Conflict => Results.Conflict(),
                    _ => Results.BadRequest()
                };
            })
            .AllowAnonymous()
            .WithTags("Users")
            .WithName("CreateUser")
            .WithSummary("Creates a new user.");

        var secured = app.MapGroup("/api/users")
            .RequireAuthorization()
            .WithTags("Users");

        secured.MapGet(string.Empty, async (IMediator mediator, CancellationToken cancellationToken) =>
            {
                var users = await mediator.Send(new GetUsersQuery(), cancellationToken);
                return Results.Ok(users);
            })
            .WithName("GetUsers")
            .WithSummary("Lists users.");

        secured.MapGet("{id:guid}", async (Guid id, IMediator mediator, CancellationToken cancellationToken) =>
            {
                var user = await mediator.Send(new GetUserByIdQuery(id), cancellationToken);
                return user is null ? Results.NotFound() : Results.Ok(user);
            })
            .WithName("GetUserById")
            .WithSummary("Gets a user by id.");

        secured.MapPut("{id:guid}", async (Guid id, UpdateUserRequest request, IMediator mediator, CancellationToken cancellationToken) =>
            {
                var result = await mediator.Send(new UpdateUserCommand(id, request.Email, request.FullName, request.Password, request.IsActive), cancellationToken);
                return result.Status switch
                {
                    OperationStatus.Success => Results.Ok(result.Value),
                    OperationStatus.NotFound => Results.NotFound(),
                    OperationStatus.Conflict => Results.Conflict(),
                    _ => Results.BadRequest()
                };
            })
            .WithName("UpdateUser")
            .WithSummary("Updates a user.");

        secured.MapDelete("{id:guid}", async (Guid id, IMediator mediator, CancellationToken cancellationToken) =>
            {
                var result = await mediator.Send(new DeleteUserCommand(id), cancellationToken);
                return result == OperationStatus.Success ? Results.NoContent() : Results.NotFound();
            })
            .WithName("DeleteUser")
            .WithSummary("Deletes a user.");

        return app;
    }
}
