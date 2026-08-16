using MilesManager.Domain.Users;
using MilesManager.Application.Abstractions;
using MilesManager.Application.Features.Users.DTOs;

namespace MilesManager.Application.Features.Users.Abstractions;

public interface IUserRepository
{
    IUnitOfWork UnitOfWork { get; }
    Task AddAsync(AppUser user, CancellationToken cancellationToken);
    Task<AppUser?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<AppUser?> GetByEmailAsync(string email, CancellationToken cancellationToken);
    Task<IReadOnlyList<AppUser>> ListAsync(CancellationToken cancellationToken);
    void Remove(AppUser user);
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
