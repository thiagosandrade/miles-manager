using MilesManager.Application.Features.Users.Abstractions;
using MilesManager.Domain.Users;
using Microsoft.EntityFrameworkCore;
using MilesManager.Application.Abstractions;

namespace MilesManager.Infrastructure.Persistence.Repositories;

public sealed class UserRepository(MilesManagerDbContext dbContext) : IUserRepository
{
    private readonly MilesManagerDbContext _dbContext = dbContext;

    public IUnitOfWork UnitOfWork => _dbContext;

    public async Task AddAsync(AppUser user, CancellationToken cancellationToken)
    {
        await _dbContext.Users.AddAsync(user, cancellationToken);
    }

    public async Task<AppUser?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _dbContext.Users
            .Include(x => x.UserRoles)
            .ThenInclude(x => x.Role)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<AppUser?> GetByEmailAsync(string email, CancellationToken cancellationToken)
    {
        var normalized = email.Trim().ToLowerInvariant();
        return await _dbContext.Users
            .Include (x => x.UserRoles)
            .ThenInclude (x => x.Role)
            .FirstOrDefaultAsync(x => x.Email == normalized, cancellationToken);
    }

    public async Task<IReadOnlyList<AppUser>> ListAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.Users
            .Include(x => x.UserRoles)
            .ThenInclude(x => x.Role)
            .AsNoTracking()
            .OrderBy(x => x.Email)
            .ToListAsync(cancellationToken);
    }

    public void Remove(AppUser user)
    {
        _dbContext.Users.Remove(user);
    }
}
