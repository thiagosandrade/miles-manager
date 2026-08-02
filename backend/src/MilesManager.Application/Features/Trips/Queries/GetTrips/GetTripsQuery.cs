using MediatR;

namespace MilesManager.Application.Features.Trips.Queries.GetTrips;

/// <summary>
/// A trip list item returned by queries.
/// </summary>
public sealed record TripListItem(Guid Id, string Name, decimal DistanceMiles, DateOnly TripDate);

/// <summary>
/// Query to list all trips.
/// </summary>
public sealed record GetTripsQuery() : IRequest<IReadOnlyList<TripListItem>>;
