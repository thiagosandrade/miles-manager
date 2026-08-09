using MediatR;
using Microsoft.AspNetCore.Mvc;
using MilesManager.Api.Common;
using MilesManager.Application.Features.Dashboard.Queries.GetDashboard;

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
    public async Task<ActionResult<IReadOnlyList<object>>> Get(CancellationToken cancellationToken)
    {
        var dashboard = await Sender.Send(new GetDashboardQuery(), cancellationToken);
        return Ok(dashboard.Programs);
    }
}
