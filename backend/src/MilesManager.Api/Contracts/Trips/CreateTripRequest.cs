namespace MilesManager.Api.Contracts.Trips;

/// <summary>
/// Request payload used to create a trip.
/// </summary>
public sealed record CreateTripRequest(string Name, decimal DistanceMiles, DateOnly TripDate);
