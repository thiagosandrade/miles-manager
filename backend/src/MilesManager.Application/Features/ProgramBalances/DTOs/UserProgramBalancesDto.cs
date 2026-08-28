#nullable enable
namespace MilesManager.Application.Features.ProgramBalances.DTOs;

public sealed class UserProgramBalancesDto
{
    public Guid ProgramId { get; init; }
    public string ProgramName { get; init; } = string.Empty;
    public decimal Balance { get; init; }
    public decimal CurrentValue { get; init; }
    public string CurrencyCode { get; set; } = string.Empty;
}
