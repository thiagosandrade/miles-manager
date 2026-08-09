using MediatR;
using MilesManager.Application.Common;
using MilesManager.Application.Features.Users.Abstractions;
using MilesManager.Application.Features.Users.Models;
using MilesManager.Domain.Users;

namespace MilesManager.Application.Features.Users.Commands.UpdateUser;

public sealed class UpdateUserHandler(IUserRepository userRepository, IPasswordHasher passwordHasher) : IRequestHandler<UpdateUserCommand, Result<UserDetails>>
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IPasswordHasher _passwordHasher = passwordHasher;

    public async Task<Result<UserDetails>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.Id, cancellationToken);
        if (user is null)
        {
            return Result<UserDetails>.NotFound();
        }

        var normalizedEmail = request.Email.Trim().ToLowerInvariant();
        var emailChanged = !string.Equals(user.Email, normalizedEmail, StringComparison.OrdinalIgnoreCase);
        if (emailChanged)
        {
            var duplicate = await _userRepository.GetByEmailAsync(normalizedEmail, cancellationToken);
            if (duplicate is not null && duplicate.Id != user.Id)
            {
                return Result<UserDetails>.Conflict();
            }
            user.Email = normalizedEmail;
        }

        var authChanged = false;
        var trimmedFullName = request.FullName.Trim();
        if (!string.Equals(user.FullName, trimmedFullName, StringComparison.Ordinal))
        {
            user.FullName = trimmedFullName;
        }

        if (user.IsActive != request.IsActive)
        {
            user.IsActive = request.IsActive;
            authChanged = true;
        }

        if (!string.IsNullOrWhiteSpace(request.Password))
        {
            user.PasswordHash = _passwordHasher.HashPassword(request.Password);
            authChanged = true;
        }

        if (emailChanged || authChanged)
        {
            user.TokenVersion += 1;
        }

        user.UpdatedAtUtc = DateTimeOffset.UtcNow;

        await _userRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        return Result<UserDetails>.Success(ToDetails(user));
    }

    private static UserDetails ToDetails(AppUser user)
    {
        return new UserDetails(user.Id, user.Email, user.FullName, user.IsActive, user.CreatedAtUtc, user.UpdatedAtUtc, user.LastLoginAtUtc);
    }
}
