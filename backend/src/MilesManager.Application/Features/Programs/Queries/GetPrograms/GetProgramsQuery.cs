#nullable enable
using MediatR;
using MilesManager.Application.Common;
using MilesManager.Application.Features.Programs.DTOs;

namespace MilesManager.Application.Features.Programs.Queries.GetPrograms;

public sealed class GetProgramsQuery : IRequest<Result<IReadOnlyList<ProgramDto>>>
{
}

