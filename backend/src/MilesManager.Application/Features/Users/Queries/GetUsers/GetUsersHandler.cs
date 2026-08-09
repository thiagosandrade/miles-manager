using MediatR;
using MilesManager.Application.Features.Users.Abstractions;
using MilesManager.Application.Features.Users.Models;

namespace MilesManager.Application.Features.Users.Queries.GetUsers;

public sealed class GetUsersHandler(IUserRepository userRepository) : IRequestHandler<GetUsersQuery, IReadOnlyList<UserSummary>>
{
    private readonly IUserRepository _userRepository = userRepository;

    public async Task<IReadOnlyList<UserSummary>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        var users = await _userRepository.ListAsync(cancellationToken);
        return users.Select(u => new UserSummary(u.Id, u.Email, u.FullName, u.IsActive, u.CreatedAtUtc)).ToList();
    }
}
