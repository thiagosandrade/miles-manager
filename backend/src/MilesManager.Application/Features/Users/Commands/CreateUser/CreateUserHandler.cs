using AutoMapper;
using MediatR;
using MilesManager.Application.Common;
using MilesManager.Application.Features.Users.Abstractions;
using MilesManager.Application.Features.Users.DTOs;
using MilesManager.Domain.Users;

namespace MilesManager.Application.Features.Users.Commands.CreateUser;

public sealed class CreateUserHandler(IUserRepository userRepository, IPasswordHasher passwordHasher, IMapper mapper) : IRequestHandler<CreateUserCommand, Result<UserDetailsDTO>>
{
    public async Task<Result<UserDetailsDTO>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();
        var existing = await userRepository.GetByEmailAsync(normalizedEmail, cancellationToken);
        if (existing is not null)
        {
            return Result<UserDetailsDTO>.Conflict();
        }

        var user = new AppUser
        {
            Email = normalizedEmail,
            FullName = request.FullName.Trim(),
            PasswordHash = passwordHasher.HashPassword(request.Password)
        };

        await userRepository.AddAsync(user, cancellationToken);
        await userRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

        return Result<UserDetailsDTO>.Success(mapper.Map<AppUser,UserDetailsDTO>(user));
    }
}
