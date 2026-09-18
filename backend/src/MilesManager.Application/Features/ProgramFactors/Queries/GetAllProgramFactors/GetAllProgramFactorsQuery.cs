using MediatR;
using MilesManager.Application.Common;
using MilesManager.Application.Features.ProgramFactors.DTOs;

namespace MilesManager.Application.Features.ProgramFactors.Queries.GetAllProgramFactors;

public sealed record GetAllProgramFactorsQuery()
    : IRequest<Result<IReadOnlyList<ProgramFactorDto>>>;
