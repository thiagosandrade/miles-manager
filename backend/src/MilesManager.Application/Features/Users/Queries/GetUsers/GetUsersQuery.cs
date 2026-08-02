using MediatR;
using MilesManager.Application.Features.Users.Models;

namespace MilesManager.Application.Features.Users.Queries.GetUsers;

public sealed record GetUsersQuery() : IRequest<IReadOnlyList<UserSummary>>;
