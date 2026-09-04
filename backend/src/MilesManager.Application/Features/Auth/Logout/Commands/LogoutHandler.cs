using MediatR;
using MilesManager.Application.Features.Users.Abstractions;
using MilesManager.Application.Features.Users.DTOs;

namespace MilesManager.Application.Features.Auth.Logout.Commands;

public sealed class LogoutHandler(IUserRepository userRepository) : IRequestHandler<LogoutCommand, OperationStatus>
{
    private readonly IUserRepository _userRepository = userRepository;

    public async Task<OperationStatus> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
        {
            return OperationStatus.NotFound;
        }

        user.TokenVersion += 1;
        user.UpdatedAtUtc = DateTimeOffset.UtcNow;
        await _userRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        return OperationStatus.Success;
    }
}
