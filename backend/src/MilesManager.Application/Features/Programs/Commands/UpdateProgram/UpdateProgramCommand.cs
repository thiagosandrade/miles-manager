#nullable enable
using MediatR;
using MilesManager.Application.Common;
using MilesManager.Domain.Common.Enums;

namespace MilesManager.Application.Features.Programs.Commands.UpdateProgram;

public sealed class UpdateProgramCommand : IRequest<Result<Guid>>
{
    public Guid Id { get; init; }
    public string ProgramCode { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public ProgramType ProgramType { get; init; }
    public string? Website { get; init; }
    public string? Logo { get; init; }
    public decimal ReferencePricePerThousand { get; init; }
    public bool IsActive { get; init; }
}
