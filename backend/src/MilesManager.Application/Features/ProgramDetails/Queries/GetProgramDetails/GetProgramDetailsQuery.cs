using MediatR;
using MilesManager.Application.Common;
using MilesManager.Application.Features.ProgramDetails.DTOs;

namespace MilesManager.Application.Features.ProgramDetails.Queries.GetProgramDetails;

public sealed record GetProgramDetailsQuery(Guid UserId, Guid ProgramId)
    : IRequest<Result<ProgramDetailsDto>>;
