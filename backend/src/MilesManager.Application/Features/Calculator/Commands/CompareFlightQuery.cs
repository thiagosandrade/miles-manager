using MediatR;
using MilesManager.Application.Features.Calculator.DTOs;

namespace MilesManager.Application.Features.Calculator.Commands
{
    public sealed record CompareFlightQuery(Guid ProgramId, decimal MilesRequired, decimal Taxes, decimal CashPrice) 
        : IRequest<FlightComparisonDto>;
}
