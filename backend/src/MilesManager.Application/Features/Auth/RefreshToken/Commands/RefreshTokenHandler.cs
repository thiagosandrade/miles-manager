using AutoMapper;
using MediatR;
using MilesManager.Application.Common;
using MilesManager.Application.Features.Users.Abstractions;
using MilesManager.Application.Features.Users.DTOs;
using MilesManager.Domain.Users;

namespace MilesManager.Application.Features.Auth.RefreshToken.Commands;

internal sealed class RefreshTokenHandler(IUserRepository userRepository, IJwtTokenService tokenService) : IRequestHandler<RefreshTokenCommand, Result<AuthSession>>
{
    public async Task<Result<AuthSession>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null || !user.IsActive)
        {
            return Result<AuthSession>.Unauthorized();
        }

        // update last login time and persist
        user.LastLoginAtUtc = DateTimeOffset.UtcNow;
        user.UpdatedAtUtc = DateTimeOffset.UtcNow;

        var token = tokenService.CreateToken(user);
        await userRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

        return Result<AuthSession>.Success(new AuthSession(token));
    }
}
