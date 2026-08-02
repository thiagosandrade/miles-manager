using MediatR;
using MilesManager.Application.Features.Trips.Abstractions;

namespace MilesManager.Application.Features.Trips.Queries.GetTrips;

/// <summary>
/// Handles listing trips.
/// </summary>
public sealed class GetTripsHandler(ITripRepository tripRepository) : IRequestHandler<GetTripsQuery, IReadOnlyList<TripListItem>>
{
    private readonly ITripRepository _tripRepository = tripRepository;

    public async Task<IReadOnlyList<TripListItem>> Handle(GetTripsQuery request, CancellationToken cancellationToken)
    {
        var trips = await _tripRepository.ListAsync(cancellationToken);
        return trips.Select(trip => new TripListItem(trip.Id, trip.Name, trip.DistanceMiles, trip.TripDate)).ToList();
    }
}
