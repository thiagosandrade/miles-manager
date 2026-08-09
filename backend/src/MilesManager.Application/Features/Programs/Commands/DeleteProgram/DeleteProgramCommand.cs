#nullable enable
using MediatR;
using MilesManager.Application.Common;

namespace MilesManager.Application.Features.Programs.Commands.DeleteProgram;

public sealed class DeleteProgramCommand : IRequest<Result>
{
    public Guid Id { get; init; }
}
