using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MilesManager.Api.Common;
using MilesManager.Application.Features.AlertSources.Commands.CreateAlertSource;
using MilesManager.Application.Features.AlertSources.Commands.DeleteAlertSource;
using MilesManager.Application.Features.AlertSources.Commands.UpdateAlertSource;
using MilesManager.Application.Features.AlertSources.DTOs;
using MilesManager.Application.Features.AlertSources.Queries.GetAlertSources;

namespace MilesManager.Api.Controllers;

[Authorize(Roles = "Administrator")]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/alert-sources")]
public sealed class AlertSourcesController(ISender sender) : ApiController(sender)
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<AlertSourceDto>>> Get(CancellationToken cancellationToken)
    {
        var result = await Sender.Send(new GetAlertSourcesQuery(), cancellationToken);
        return Convert(result);
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> Create(
        [FromBody] CreateAlertSourceCommand command,
        CancellationToken cancellationToken)
    {
        var result = await Sender.Send(command, cancellationToken);
        return Convert(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateAlertSourceCommand command,
        CancellationToken cancellationToken)
    {
        if (id != command.Id)
            return BadRequest();

        var result = await Sender.Send(command, cancellationToken);
        return Convert(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await Sender.Send(new DeleteAlertSourceCommand(id), cancellationToken);
        return Convert(result);
    }
}
