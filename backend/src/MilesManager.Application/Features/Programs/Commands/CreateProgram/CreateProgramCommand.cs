#nullable enable
using MediatR;
using MilesManager.Application.Common;
using MilesManager.Domain.Common.Enums;

namespace MilesManager.Application.Features.Programs.Commands.CreateProgram;

public sealed class CreateProgramCommand : IRequest<Result<Guid>>
{
    public string ProgramCode { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public ProgramType ProgramType { get; init; }
    public string? Website { get; init; }
    public string? Logo { get; init; }
    public decimal ReferencePricePerThousand { get; init; }
    public bool IsActive { get; init; } = true;
    public string CurrencyCode { get; init; } = string.Empty;
}
