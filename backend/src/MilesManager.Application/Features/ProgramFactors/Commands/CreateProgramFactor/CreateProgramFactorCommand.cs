using MediatR;
using MilesManager.Application.Common;

namespace MilesManager.Application.Features.ProgramFactors.Commands.CreateProgramFactor;

public sealed class CreateProgramFactorCommand : IRequest<Result<Guid>>
{
    public Guid OriginProgramId { get; init; }
    public Guid DestinationProgramId { get; init; }
    public decimal FromAmount { get; init; }
    public decimal ToAmount { get; init; }
}
