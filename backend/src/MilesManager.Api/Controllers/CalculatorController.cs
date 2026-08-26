using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MilesManager.Api.Common;
using MilesManager.Application.Features.Calculator.Commands;
using MilesManager.Application.Features.Calculator.DTOs;

namespace MilesManager.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/v{version:apiVersion}/calculator")]
[ApiVersion("1.0")]
public sealed class CalculatorController(ISender sender) : ApiController(sender)
{
    [HttpPost("compare-flight")]
    public async Task<ActionResult<FlightComparisonDto>> CompareFlight(
        [FromBody] CompareFlightQuery query,
        CancellationToken cancellationToken)
    {
        var result = await Sender.Send(query, cancellationToken);

        return Ok(result);
    }
}
