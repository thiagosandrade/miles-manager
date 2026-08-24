using AutoMapper;
using MediatR;
using MilesManager.Application.Common;
using MilesManager.Application.Features.Users.Abstractions;
using MilesManager.Application.Features.Users.DTOs;

namespace MilesManager.Application.Features.Auth.Login;

public sealed class LoginCommandHandler(IUserRepository userRepository, IPasswordHasher passwordHasher, IJwtTokenService tokenService) : IRequestHandler<LoginCommand, Result<AuthSession>>
{
    public async Task<Result<AuthSession>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByEmailAsync(request.Email.Trim().ToLowerInvariant(), cancellationToken);
        if (user is null || !user.IsActive)
        {
            return Result<AuthSession>.Unauthorized();
        }

        if (!passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
        {
            return Result<AuthSession>.Unauthorized();
        }

        user.LastLoginAtUtc = DateTimeOffset.UtcNow;
        user.UpdatedAtUtc = DateTimeOffset.UtcNow;

        var token = tokenService.CreateToken(user);
        await userRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

        return Result<AuthSession>.Success(new AuthSession(token));
    }
}
