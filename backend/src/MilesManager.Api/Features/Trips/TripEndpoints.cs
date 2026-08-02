using MediatR;
using MilesManager.Api.Contracts.Trips;
using MilesManager.Application.Features.Trips.Commands.CreateTrip;
using MilesManager.Application.Features.Trips.Queries.GetTrips;

namespace MilesManager.Api.Features.Trips;

/// <summary>
/// TripEndpoints
/// </summary>
public static class TripEndpoints
{
    /// <summary>
    /// Maps trip CRUD endpoints.
    /// </summary>
    public static IEndpointRouteBuilder MapTripEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/trips").WithTags("Trips");

        group.MapGet(string.Empty, async (IMediator mediator, CancellationToken cancellationToken) =>
            {
                var trips = await mediator.Send(new GetTripsQuery(), cancellationToken);
                return Results.Ok(trips);
            })
            .WithName("GetTrips")
            .WithSummary("Gets all trips.")
            .Produces(StatusCodes.Status200OK);

        group.MapPost(string.Empty, async (CreateTripRequest request, IMediator mediator, CancellationToken cancellationToken) =>
            {
                var id = await mediator.Send(new CreateTripCommand(request.Name, request.DistanceMiles, request.TripDate), cancellationToken);
                return Results.Created($"/api/trips/{id}", new { id });
            })
            .WithName("CreateTrip")
            .WithSummary("Creates a trip.")
            .Produces(StatusCodes.Status201Created);

        return app;
    }
}
