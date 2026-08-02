using MilesManager.Domain.Trips;
using Microsoft.EntityFrameworkCore;
using MilesManager.Application.Features.Trips.Abstractions;

namespace MilesManager.Infrastructure.Persistence;

public sealed class TripRepository(MilesManagerDbContext dbContext) : ITripRepository
{
    private readonly MilesManagerDbContext _dbContext = dbContext;

    public IUnitOfWork UnitOfWork => _dbContext;

    public async Task AddAsync(Trip trip, CancellationToken cancellationToken)
    {
        await _dbContext.Trips.AddAsync(trip, cancellationToken);
    }

    public async Task<IReadOnlyList<Trip>> ListAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.Trips.AsNoTracking().OrderByDescending(x => x.TripDate).ToListAsync(cancellationToken);
    }
}
