using MilesManager.Domain.Users;
using MilesManager.Application.Features.Users.Models;

namespace MilesManager.Application.Features.Users.Abstractions;

public interface IUserRepository
{
    IUserUnitOfWork UnitOfWork { get; }
    Task AddAsync(AppUser user, CancellationToken cancellationToken);
    Task<AppUser?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<AppUser?> GetByEmailAsync(string email, CancellationToken cancellationToken);
    Task<IReadOnlyList<AppUser>> ListAsync(CancellationToken cancellationToken);
    void Remove(AppUser user);
}

public interface IUserUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}

public interface IPasswordHasher
{
    string HashPassword(string password);
    bool VerifyPassword(string password, string passwordHash);
}

public interface IJwtTokenService
{
    IssuedToken CreateToken(AppUser user);
}
