using MilesManager.Domain.Trips;

namespace MilesManager.Application.Features.Trips.Abstractions;

/// <summary>
/// Trip repository contract used by the application layer.
/// </summary>
public interface ITripRepository
{
    IUnitOfWork UnitOfWork { get; }
    Task AddAsync(Trip trip, CancellationToken cancellationToken);
    Task<IReadOnlyList<Trip>> ListAsync(CancellationToken cancellationToken);
}

/// <summary>
/// Minimal unit of work abstraction for persistence.
/// </summary>
public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
