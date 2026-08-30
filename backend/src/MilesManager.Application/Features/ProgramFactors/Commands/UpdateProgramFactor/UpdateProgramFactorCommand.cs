using MediatR;
using MilesManager.Application.Common;

namespace MilesManager.Application.Features.ProgramFactors.Commands.UpdateProgramFactor;

public sealed class UpdateProgramFactorCommand : IRequest<Result<Guid>>
{
    public Guid Id { get; init; }
    public Guid OriginProgramId { get; init; }
    public Guid DestinationProgramId { get; init; }
    public decimal TransferFactor { get; init; }
}
