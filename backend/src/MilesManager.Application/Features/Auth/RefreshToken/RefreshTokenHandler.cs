using MediatR;
using MilesManager.Application.Common;
using MilesManager.Application.Features.Users.Abstractions;
using MilesManager.Application.Features.Users.Models;

namespace MilesManager.Application.Features.Auth.RefreshToken;

internal sealed class RefreshTokenHandler(IUserRepository userRepository, IJwtTokenService tokenService) : IRequestHandler<RefreshTokenCommand, Result<AuthSession>>
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IJwtTokenService _tokenService = tokenService;

    public async Task<Result<AuthSession>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null || !user.IsActive)
        {
            return Result<AuthSession>.Unauthorized();
        }

        // update last login time and persist
        user.LastLoginAtUtc = DateTimeOffset.UtcNow;
        user.UpdatedAtUtc = DateTimeOffset.UtcNow;

        var token = _tokenService.CreateToken(user);
        await _userRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

        var summary = new UserSummary(user.Id, user.Email, user.FullName, user.IsActive, user.CreatedAtUtc);
        return Result<AuthSession>.Success(new AuthSession(token, summary));
    }
}
