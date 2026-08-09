using MediatR;
using MilesManager.Application.Features.Users.Abstractions;
using MilesManager.Application.Features.Users.Models;

namespace MilesManager.Application.Features.Users.Queries.GetUserById;

public sealed class GetUserByIdHandler(IUserRepository userRepository) : IRequestHandler<GetUserByIdQuery, UserDetails?>
{
    private readonly IUserRepository _userRepository = userRepository;

    public async Task<UserDetails?> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.Id, cancellationToken);
        return user is null ? null : new UserDetails(user.Id, user.Email, user.FullName, user.IsActive, user.CreatedAtUtc, user.UpdatedAtUtc, user.LastLoginAtUtc);
    }
}
