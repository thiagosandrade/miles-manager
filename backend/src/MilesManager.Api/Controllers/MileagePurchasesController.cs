using MediatR;
using Microsoft.AspNetCore.Mvc;
using MilesManager.Api.Common;
using MilesManager.Application.Features.Purchases.Commands.CreateMileagePurchase;
using MilesManager.Application.Features.Purchases.Commands.UpdateMileagePurchase;
using MilesManager.Application.Features.Purchases.Commands.DeleteMileagePurchase;
using MilesManager.Application.Features.Purchases.Queries.GetPurchases;
using MilesManager.Application.Features.Purchases.Queries.GetMileagePurchaseById;

namespace MilesManager.Api.Controllers;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/mileage-purchases")]
public sealed class MileagePurchasesController(ISender sender) : ApiController(sender)
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<Application.Features.Purchases.DTOs.MileagePurchaseDto>>> Get(
        CancellationToken cancellationToken)
    {
        var result = await Sender.Send(new GetPurchasesQuery(), cancellationToken);
        return Convert(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Application.Features.Purchases.DTOs.MileagePurchaseDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await Sender.Send(new GetMileagePurchaseByIdQuery { Id = id }, cancellationToken);
        return Convert(result);
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> Create([FromBody] CreateMileagePurchaseCommand command, CancellationToken cancellationToken)
    {
        var created = new CreateMileagePurchaseCommand
        {
            UserId = GetUserId(),
            ProgramId = command.ProgramId,
            Partner = command.Partner,
            Description = command.Description,
            AmountPaid = command.AmountPaid,
            MilesPurchased = command.MilesPurchased,
            PurchaseDate = command.PurchaseDate,
            PurchaseStatus = command.PurchaseStatus
        };
        var result = await Sender.Send(created, cancellationToken);
        return Convert(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<Guid>> Update(Guid id, [FromBody] UpdateMileagePurchaseCommand command, CancellationToken cancellationToken)
    {
        var updated = new UpdateMileagePurchaseCommand
        {
            Id = id,
            UserId = GetUserId(),
            ProgramId = command.ProgramId,
            Partner = command.Partner,
            Description = command.Description,
            AmountPaid = command.AmountPaid,
            MilesPurchased = command.MilesPurchased,
            PurchaseDate = command.PurchaseDate,
            PurchaseStatus = command.PurchaseStatus
        };
        var result = await Sender.Send(updated, cancellationToken);
        return Convert(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await Sender.Send(new DeleteMileagePurchaseCommand { Id = id }, cancellationToken);
        return Convert(result);
    }
}
