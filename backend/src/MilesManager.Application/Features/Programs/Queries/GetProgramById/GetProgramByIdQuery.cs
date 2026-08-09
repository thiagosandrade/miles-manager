#nullable enable
using MediatR;
using MilesManager.Application.Common;
using MilesManager.Application.Features.Programs.DTOs;

namespace MilesManager.Application.Features.Programs.Queries.GetProgramById;

public sealed class GetProgramByIdQuery : IRequest<Result<ProgramDto>>
{
    public Guid Id { get; init; }
}
