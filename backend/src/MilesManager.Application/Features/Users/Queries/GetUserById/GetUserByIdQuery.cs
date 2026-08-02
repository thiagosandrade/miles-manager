using MediatR;
using MilesManager.Application.Features.Users.Models;

namespace MilesManager.Application.Features.Users.Queries.GetUserById;

public sealed record GetUserByIdQuery(Guid Id) : IRequest<UserDetails?>;
