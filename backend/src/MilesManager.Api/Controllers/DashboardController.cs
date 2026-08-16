using MediatR;
using Microsoft.AspNetCore.Mvc;
using MilesManager.Api.Common;
using MilesManager.Application.Features.Dashboard.Queries.GetDashboard;

namespace MilesManager.Api.Controllers;

/// <summary>
/// Controller to expose dashboard data.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="DashboardController"/> class.
/// </remarks>
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/dashboard")]
public sealed class DashboardController(ISender sender) : ApiController(sender)
{

    /// <summary>
    /// Gets dashboard summary.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<Application.Features.Dashboard.DTOs.DashboardDto>> Get(CancellationToken cancellationToken)
    {
        var dto = await Sender.Send(new GetDashboardQuery(), cancellationToken);
        return Ok(dto);
    }
}
