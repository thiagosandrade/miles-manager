using MediatR;
using MilesManager.Application.Abstractions;
using MilesManager.Application.Common;
using MilesManager.Application.Features.Users.Abstractions;

namespace MilesManager.Application.Features.Auth.ForgotPassword;

public sealed class ResetPasswordCommandHandler(
    IUserRepository userRepository,
    IPasswordResetTokenService tokenService,
    IPasswordHasher passwordHasher) : IRequestHandler<ResetPasswordCommand, Result>
{
    public async Task<Result> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        if (!tokenService.TryValidateToken(request.Token, out var tokenData))
        {
            return Result.Unauthorized();
        }

        var user = await userRepository.GetByIdAsync(tokenData.UserId, cancellationToken);
        if (user is null || !user.IsActive || user.TokenVersion != tokenData.TokenVersion)
        {
            return Result.Unauthorized();
        }

        user.PasswordHash = passwordHasher.HashPassword(request.NewPassword);
        user.TokenVersion += 1;
        user.UpdatedAtUtc = DateTimeOffset.UtcNow;

        await userRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
