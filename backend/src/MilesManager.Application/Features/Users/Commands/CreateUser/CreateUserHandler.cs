using MediatR;
using MilesManager.Application.Features.Users.Abstractions;
using MilesManager.Application.Features.Users.Models;
using MilesManager.Domain.Users;

namespace MilesManager.Application.Features.Users.Commands.CreateUser;

public sealed class CreateUserHandler(IUserRepository userRepository, IPasswordHasher passwordHasher) : IRequestHandler<CreateUserCommand, OperationResult<UserDetails>>
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IPasswordHasher _passwordHasher = passwordHasher;

    public async Task<OperationResult<UserDetails>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();
        var existing = await _userRepository.GetByEmailAsync(normalizedEmail, cancellationToken);
        if (existing is not null)
        {
            return OperationResult<UserDetails>.Conflict();
        }

        var user = new AppUser
        {
            Email = normalizedEmail,
            FullName = request.FullName.Trim(),
            PasswordHash = _passwordHasher.HashPassword(request.Password)
        };

        await _userRepository.AddAsync(user, cancellationToken);
        await _userRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

        return OperationResult<UserDetails>.Success(ToDetails(user));
    }

    private static UserDetails ToDetails(AppUser user)
    {
        return new UserDetails(user.Id, user.Email, user.FullName, user.IsActive, user.CreatedAtUtc, user.UpdatedAtUtc, user.LastLoginAtUtc);
    }
}
