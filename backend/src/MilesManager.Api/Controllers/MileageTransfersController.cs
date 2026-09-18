using MediatR;
using Microsoft.AspNetCore.Mvc;
using MilesManager.Api.Common;
using MilesManager.Application.Features.Transfers.Commands.CreateTransfer;
using MilesManager.Application.Features.Transfers.Commands.UpdateMileageTransfer;
using MilesManager.Application.Features.Transfers.Commands.DeleteMileageTransfer;
using MilesManager.Application.Features.Transfers.Queries.GetTransfers;
using MilesManager.Application.Features.Transfers.Queries.GetMileageTransferById;

namespace MilesManager.Api.Controllers;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/mileage-transfers")]
public sealed class MileageTransfersController(ISender sender) : ApiController(sender)
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<Application.Features.Transfers.DTOs.MileageTransferDto>>> Get(
        CancellationToken cancellationToken)
    {
        var result = await Sender.Send(new GetTransfersQuery(), cancellationToken);
        return Convert(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Application.Features.Transfers.DTOs.MileageTransferDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await Sender.Send(new GetMileageTransferByIdQuery { Id = id }, cancellationToken);
        return Convert(result);
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> Create([FromBody] CreateTransferCommand command, CancellationToken cancellationToken)
    {
        var created = new CreateTransferCommand
        {
            UserId = GetUserId(), OriginProgramId = command.OriginProgramId, DestinationProgramId = command.DestinationProgramId,
            MilesSent = command.MilesSent, BonusPercentage = command.BonusPercentage, MilesReceived = command.MilesReceived,
            FactorFrom = command.FactorFrom, FactorTo = command.FactorTo, TransferDate = command.TransferDate
        };
        var result = await Sender.Send(created, cancellationToken);
        return Convert(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<Guid>> Update(Guid id, [FromBody] UpdateMileageTransferCommand command, CancellationToken cancellationToken)
    {
        var updated = new UpdateMileageTransferCommand
        {
            Id = id, UserId = GetUserId(), OriginProgramId = command.OriginProgramId, DestinationProgramId = command.DestinationProgramId,
            MilesSent = command.MilesSent, BonusPercentage = command.BonusPercentage, MilesReceived = command.MilesReceived,
            FactorFrom = command.FactorFrom, FactorTo = command.FactorTo, TransferDate = command.TransferDate
        };
        var result = await Sender.Send(updated, cancellationToken);
        return Convert(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await Sender.Send(new DeleteMileageTransferCommand { Id = id }, cancellationToken);
        return Convert(result);
    }
}
