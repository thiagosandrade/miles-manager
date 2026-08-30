using MediatR;
using MilesManager.Application.Common;

namespace MilesManager.Application.Features.ProgramFactors.Commands.DeleteProgramFactor;

public sealed class DeleteProgramFactorCommand : IRequest<Result>
{
    public Guid Id { get; init; }
}
