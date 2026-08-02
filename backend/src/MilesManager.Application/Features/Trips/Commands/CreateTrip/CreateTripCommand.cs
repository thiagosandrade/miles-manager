using MediatR;
using MilesManager.Domain.Trips;

namespace MilesManager.Application.Features.Trips.Commands.CreateTrip;

/// <summary>
/// Command to create a new trip.
/// </summary>
public sealed record CreateTripCommand(string Name, decimal DistanceMiles, DateOnly TripDate) : IRequest<Guid>;
