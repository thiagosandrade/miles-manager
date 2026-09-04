using MediatR;
using MilesManager.Application.Abstractions;
using MilesManager.Application.Common;
using MilesManager.Application.Features.Users.Abstractions;

namespace MilesManager.Application.Features.Auth.ForgotPassword;

public sealed class ValidatePasswordResetTokenQueryHandler(
    IUserRepository userRepository,
    IPasswordResetTokenService tokenService) : IRequestHandler<ValidatePasswordResetTokenQuery, Result<PasswordResetTokenValidation>>
{
    public async Task<Result<PasswordResetTokenValidation>> Handle(
        ValidatePasswordResetTokenQuery request,
        CancellationToken cancellationToken)
    {
        if (!tokenService.TryValidateToken(request.Token, out var tokenData))
        {
            return Result<PasswordResetTokenValidation>.Success(new(false, null));
        }

        var user = await userRepository.GetByIdAsync(tokenData.UserId, cancellationToken);
        if (user is null || !user.IsActive || user.TokenVersion != tokenData.TokenVersion)
        {
            return Result<PasswordResetTokenValidation>.Success(new(false, null));
        }

        return Result<PasswordResetTokenValidation>.Success(new(true, user.Email));
    }
}
