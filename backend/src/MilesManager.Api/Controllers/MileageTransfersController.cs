using MediatR;
using Microsoft.AspNetCore.Mvc;
using MilesManager.Api.Common;
using MilesManager.Application.Features.Transfers.Commands.CreateTransfer;
using MilesManager.Application.Features.Transfers.Commands.UpdateMileageTransfer;
using MilesManager.Application.Features.Transfers.Commands.DeleteMileageTransfer;
using MilesManager.Application.Features.Transfers.Queries.GetTransfers;
using MilesManager.Application.Features.Transfers.Queries.GetMileageTransferById;

namespace MilesManager.Api.Controllers;

/// <summary>
/// Controller for mileage transfers.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="MileageTransfersController"/> class.
/// </remarks>
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/mileage-transfers")]
public sealed class MileageTransfersController(ISender sender) : ApiController(sender)
{

    /// <summary>
    /// Gets mileage transfers.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<MilesManager.Application.Features.Transfers.DTOs.MileageTransferDto>>> Get(CancellationToken cancellationToken)
    {
        var result = await Sender.Send(new GetTransfersQuery(), cancellationToken);
        return Convert(result);
    }

    /// <summary>
    /// Gets a mileage transfer by id.
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<MilesManager.Application.Features.Transfers.DTOs.MileageTransferDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await Sender.Send(new GetMileageTransferByIdQuery { Id = id }, cancellationToken);
        return Convert(result);
    }

    /// <summary>
    /// Creates a mileage transfer for the authenticated user.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<Guid>> Create([FromBody] CreateTransferCommand command, CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        var created = new CreateTransferCommand
        {
            UserId = userId,
            OriginProgramId = command.OriginProgramId,
            DestinationProgramId = command.DestinationProgramId,
            MilesSent = command.MilesSent,
            BonusPercentage = command.BonusPercentage,
            MilesReceived = command.MilesReceived,
            CostPerThousandOrigin = command.CostPerThousandOrigin,
            CostPerThousandDestination = command.CostPerThousandDestination,
            TransferDate = command.TransferDate
        };
        var result = await Sender.Send(created, cancellationToken);
        return Convert(result);
    }

    /// <summary>
    /// Updates an existing mileage transfer.
    /// </summary>
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<Guid>> Update(Guid id, [FromBody] UpdateMileageTransferCommand command, CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        var updated = new UpdateMileageTransferCommand
        {
            Id = id,
            UserId = userId,
            OriginProgramId = command.OriginProgramId,
            DestinationProgramId = command.DestinationProgramId,
            MilesSent = command.MilesSent,
            BonusPercentage = command.BonusPercentage,
            MilesReceived = command.MilesReceived,
            CostPerThousandOrigin = command.CostPerThousandOrigin,
            CostPerThousandDestination = command.CostPerThousandDestination,
            TransferDate = command.TransferDate
        };
        var result = await Sender.Send(updated, cancellationToken);
        return Convert(result);
    }

    /// <summary>
    /// Deletes a mileage transfer.
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await Sender.Send(new DeleteMileageTransferCommand { Id = id }, cancellationToken);
        return Convert(result);
    }
}
