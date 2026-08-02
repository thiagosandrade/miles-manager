using MediatR;
using MilesManager.Application.Features.Trips.Abstractions;
using MilesManager.Domain.Trips;

namespace MilesManager.Application.Features.Trips.Commands.CreateTrip;

/// <summary>
/// Handles creating a new trip.
/// </summary>
public sealed class CreateTripHandler(ITripRepository tripRepository) : IRequestHandler<CreateTripCommand, Guid>
{
    private readonly ITripRepository _tripRepository = tripRepository;

    public async Task<Guid> Handle(CreateTripCommand request, CancellationToken cancellationToken)
    {
        var trip = new Trip
        {
            Name = request.Name,
            DistanceMiles = request.DistanceMiles,
            TripDate = request.TripDate
        };

        await _tripRepository.AddAsync(trip, cancellationToken);
        await _tripRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

        return trip.Id;
    }
}
