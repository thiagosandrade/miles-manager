using MediatR;
using MilesManager.Application.Features.Users.DTOs;

namespace MilesManager.Application.Features.Users.Queries.GetUsers;

public sealed record GetUsersQuery() : IRequest<IReadOnlyList<UserDTO>>;
