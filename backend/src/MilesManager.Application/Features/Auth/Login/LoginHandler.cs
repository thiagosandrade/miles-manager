using MediatR;
using MilesManager.Application.Common;
using MilesManager.Application.Features.Users.Abstractions;
using MilesManager.Application.Features.Users.Models;

namespace MilesManager.Application.Features.Auth.Login;

public sealed class LoginHandler(IUserRepository userRepository, IPasswordHasher passwordHasher, IJwtTokenService tokenService) : IRequestHandler<LoginCommand, Result<AuthSession>>
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IPasswordHasher _passwordHasher = passwordHasher;
    private readonly IJwtTokenService _tokenService = tokenService;

    public async Task<Result<AuthSession>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email.Trim().ToLowerInvariant(), cancellationToken);
        if (user is null || !user.IsActive)
        {
            return Result<AuthSession>.Unauthorized();
        }

        if (!_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
        {
            return Result<AuthSession>.Unauthorized();
        }

        user.LastLoginAtUtc = DateTimeOffset.UtcNow;
        user.UpdatedAtUtc = DateTimeOffset.UtcNow;

        var token = _tokenService.CreateToken(user);
        await _userRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

        var summary = new UserSummary(user.Id, user.Email, user.FullName, user.IsActive, user.CreatedAtUtc);
        return Result<AuthSession>.Success(new AuthSession(token, summary));
    }
}
