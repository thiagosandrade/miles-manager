#nullable enable
using MilesManager.Domain.Common.Enums;

namespace MilesManager.Application.Features.Programs.DTOs;

public sealed class ProgramDto
{
    public Guid Id { get; init; }
    public string Code { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public ProgramType ProgramType { get; init; }
    public string? Website { get; init; }
    public string? Logo { get; init; }
    public decimal ReferencePricePerThousand { get; init; }
    public bool IsActive { get; init; }
}
