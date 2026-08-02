using MediatR;
using MilesManager.Application.Features.Users.Abstractions;
using MilesManager.Application.Features.Users.Models;

namespace MilesManager.Application.Features.Users.Commands.Login;

public sealed class LoginHandler(IUserRepository userRepository, IPasswordHasher passwordHasher, IJwtTokenService tokenService) : IRequestHandler<LoginCommand, OperationResult<AuthSession>>
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IPasswordHasher _passwordHasher = passwordHasher;
    private readonly IJwtTokenService _tokenService = tokenService;

    public async Task<OperationResult<AuthSession>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email.Trim().ToLowerInvariant(), cancellationToken);
        if (user is null || !user.IsActive)
        {
            return OperationResult<AuthSession>.Unauthorized();
        }

        if (!_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
        {
            return OperationResult<AuthSession>.Unauthorized();
        }

        user.LastLoginAtUtc = DateTimeOffset.UtcNow;
        user.UpdatedAtUtc = DateTimeOffset.UtcNow;

        var token = _tokenService.CreateToken(user);
        await _userRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

        var summary = new UserSummary(user.Id, user.Email, user.FullName, user.IsActive, user.CreatedAtUtc);
        return OperationResult<AuthSession>.Success(new AuthSession(token, summary));
    }
}
