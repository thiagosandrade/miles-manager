using AutoMapper;
using MediatR;
using MilesManager.Application.Common;
using MilesManager.Application.Features.Users.Abstractions;
using MilesManager.Application.Features.Users.DTOs;
using MilesManager.Domain.Users;

namespace MilesManager.Application.Features.Users.Commands.UpdateUser;

public sealed class UpdateUserCommandHandler(IUserRepository userRepository, IPasswordHasher passwordHasher, IMapper mapper) : IRequestHandler<UpdateUserCommand, Result<UserDetailsDTO>>
{
    public async Task<Result<UserDetailsDTO>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(request.Id, cancellationToken);
        if (user is null)
        {
            return Result<UserDetailsDTO>.NotFound();
        }

        var normalizedEmail = request.Email.Trim().ToLowerInvariant();
        var emailChanged = !string.Equals(user.Email, normalizedEmail, StringComparison.OrdinalIgnoreCase);
        if (emailChanged)
        {
            var duplicate = await userRepository.GetByEmailAsync(normalizedEmail, cancellationToken);
            if (duplicate is not null && duplicate.Id != user.Id)
            {
                return Result<UserDetailsDTO>.Conflict();
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
            user.PasswordHash = passwordHasher.HashPassword(request.Password);
            authChanged = true;
        }

        if (emailChanged || authChanged)
        {
            user.TokenVersion += 1;
        }

        user.UpdatedAtUtc = DateTimeOffset.UtcNow;

        await userRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

        return Result<UserDetailsDTO>.Success(mapper.Map<AppUser, UserDetailsDTO>(user));
    }
}
