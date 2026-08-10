using MediatR;
using Microsoft.AspNetCore.Mvc;
using MilesManager.Api.Common;
using MilesManager.Application.Features.Lookup.DTOs;
using MilesManager.Application.Features.Lookup.Queries.GetLookup;

namespace MilesManager.Api.Controllers;

/// <summary>
/// Controller to expose dashboard data.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="DashboardController"/> class.
/// </remarks>
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/lookup")]
public sealed class LookupController(ISender sender) : ApiController(sender)
{

    /// <summary>
    /// Gets dashboard summary.
    /// </summary>
    [HttpGet("airports")]
    public async Task<ActionResult<LookupDto>> Get(CancellationToken cancellationToken)
    {
        var dto = await Sender.Send(new GetLookupQuery(), cancellationToken);
        return Ok(dto);
    }
}
