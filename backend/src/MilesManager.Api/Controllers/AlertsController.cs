using MediatR;
using Microsoft.AspNetCore.Mvc;
using MilesManager.Api.Common;
using Microsoft.AspNetCore.Authorization;
using MilesManager.Application.Features.Alerts.Commands.DeleteAlertItem;
using MilesManager.Application.Features.Alerts.DTOs;
using MilesManager.Application.Features.Alerts.Queries.GetAlertItems;

namespace MilesManager.Api.Controllers;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/alerts")]
public sealed class AlertsController(ISender sender) : ApiController(sender)
{
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await Sender.Send(new DeleteAlertItemCommand(id), cancellationToken);
        return Convert(result);
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<AlertItemDto>>> Get(CancellationToken cancellationToken)
    {
        var result = await Sender.Send(new GetAlertItemsQuery(), cancellationToken);
        return Convert(result);
    }
}
