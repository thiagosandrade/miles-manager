using MediatR;
using MilesManager.Application.Features.Users.Abstractions;
using MilesManager.Application.Features.Users.DTOs;

namespace MilesManager.Application.Features.Users.Commands.DeleteUser;

public sealed class DeleteUserHandler(IUserRepository userRepository) : IRequestHandler<DeleteUserCommand, OperationStatus>
{
    private readonly IUserRepository _userRepository = userRepository;

    public async Task<OperationStatus> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.Id, cancellationToken);
        if (user is null)
        {
            return OperationStatus.NotFound;
        }

        _userRepository.Remove(user);
        await _userRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        return OperationStatus.Success;
    }
}
