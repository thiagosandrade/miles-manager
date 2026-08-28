using MediatR;
using Microsoft.AspNetCore.Mvc;
using MilesManager.Api.Common;
using MilesManager.Application.Features.ProgramBalances.DTOs;
using MilesManager.Application.Features.ProgramBalances.Queries.GetUserProgramPurchases;

namespace MilesManager.Api.Controllers;

/// <summary>
/// Controller to expose program balances.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="ProgramBalancesController"/> class.
/// </remarks>
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/program-balances")]
public sealed class ProgramBalancesController(ISender sender) : ApiController(sender)
{

    /// <summary>
    /// Gets program balances summary from the dashboard.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<UserProgramBalancesDto>>> Get(CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        var result = await Sender.Send(new GetUserProgramBalancesQuery(userId), cancellationToken);
        return Ok(result.Value);
    }
}
