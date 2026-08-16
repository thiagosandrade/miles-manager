using MediatR;
using Microsoft.AspNetCore.Mvc;
using MilesManager.Api.Common;
using MilesManager.Application.Features.Purchases.Commands.CreateMileagePurchase;
using MilesManager.Application.Features.Purchases.Commands.UpdateMileagePurchase;
using MilesManager.Application.Features.Purchases.Commands.DeleteMileagePurchase;
using MilesManager.Application.Features.Purchases.Queries.GetPurchases;
using MilesManager.Application.Features.Purchases.Queries.GetMileagePurchaseById;

namespace MilesManager.Api.Controllers;

/// <summary>
/// Controller for managing mileage purchases.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="MileagePurchasesController"/> class.
/// </remarks>
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/mileage-purchases")]
public sealed class MileagePurchasesController(ISender sender) : ApiController(sender)
{

    /// <summary>
    /// Gets mileage purchases.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<Application.Features.Purchases.DTOs.MileagePurchaseDto>>> Get(CancellationToken cancellationToken)
    {
        var result = await Sender.Send(new GetPurchasesQuery(), cancellationToken);
        return Convert(result);
    }

    /// <summary>
    /// Gets a mileage purchase by id.
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Application.Features.Purchases.DTOs.MileagePurchaseDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await Sender.Send(new GetMileagePurchaseByIdQuery { Id = id }, cancellationToken);
        return Convert(result);
    }

    /// <summary>
    /// Creates a new mileage purchase for the authenticated user.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<Guid>> Create([FromBody] CreateMileagePurchaseCommand command, CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        var created = new CreateMileagePurchaseCommand
        {
            UserId = userId,
            ProgramId = command.ProgramId,
            Partner = command.Partner,
            Description = command.Description,
            AmountPaid = command.AmountPaid,
            MilesPurchased = command.MilesPurchased,
            BonusPercentage = command.BonusPercentage,
            PurchaseDate = command.PurchaseDate
        };
        var result = await Sender.Send(created, cancellationToken);
        return Convert(result);
    }

    /// <summary>
    /// Updates an existing mileage purchase.
    /// </summary>
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<Guid>> Update(Guid id, [FromBody] UpdateMileagePurchaseCommand command, CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        var updated = new UpdateMileagePurchaseCommand
        {
            Id = id,
            UserId = userId,
            ProgramId = command.ProgramId,
            Partner = command.Partner,
            Description = command.Description,
            AmountPaid = command.AmountPaid,
            MilesPurchased = command.MilesPurchased,
            BonusPercentage = command.BonusPercentage,
            PurchaseDate = command.PurchaseDate
        };
        var result = await Sender.Send(updated, cancellationToken);
        return Convert(result);
    }

    /// <summary>
    /// Deletes a mileage purchase.
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await Sender.Send(new DeleteMileagePurchaseCommand { Id = id }, cancellationToken);
        return Convert(result);
    }
}
