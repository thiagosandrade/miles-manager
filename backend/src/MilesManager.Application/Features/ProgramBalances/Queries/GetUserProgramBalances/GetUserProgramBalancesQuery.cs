using MediatR;
using MilesManager.Application.Common;
using MilesManager.Application.Features.ProgramBalances.DTOs;

namespace MilesManager.Application.Features.ProgramBalances.Queries.GetUserProgramPurchases;

public sealed record GetUserProgramBalancesQuery(Guid UserId) : IRequest<Result<IReadOnlyList<UserProgramBalancesDto>>>;
