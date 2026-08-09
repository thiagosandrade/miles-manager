using MediatR;
using Microsoft.AspNetCore.Mvc;
using MilesManager.Api.Common;
using MilesManager.Application.Features.Redemptions.Commands.CreateRedemption;
using MilesManager.Application.Features.Redemptions.Commands.UpdateFlightRedemption;
using MilesManager.Application.Features.Redemptions.Commands.DeleteFlightRedemption;
using MilesManager.Application.Features.Redemptions.Queries.GetRedemptions;
using MilesManager.Application.Features.Redemptions.Queries.GetFlightRedemptionById;

namespace MilesManager.Api.Controllers;

/// <summary>
/// Controller for flight redemptions.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="FlightRedemptionsController"/> class.
/// </remarks>
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/flight-redemptions")]
public sealed class FlightRedemptionsController(ISender sender) : ApiController(sender)
{

    /// <summary>
    /// Gets flight redemptions.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<MilesManager.Application.Features.Redemptions.DTOs.FlightRedemptionDto>>> Get(CancellationToken cancellationToken)
    {
        var result = await Sender.Send(new GetRedemptionsQuery(), cancellationToken);
        return Convert(result);
    }

    /// <summary>
    /// Gets a flight redemption by id.
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<MilesManager.Application.Features.Redemptions.DTOs.FlightRedemptionDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await Sender.Send(new GetFlightRedemptionByIdQuery { Id = id }, cancellationToken);
        return Convert(result);
    }

    /// <summary>
    /// Creates a flight redemption for the authenticated user.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<Guid>> Create([FromBody] CreateRedemptionCommand command, CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        var created = new CreateRedemptionCommand
        {
            UserId = userId,
            ProgramId = command.ProgramId,
            OriginAirportCode = command.OriginAirportCode,
            DestinationAirportCode = command.DestinationAirportCode,
            FlightDate = command.FlightDate,
            CabinClass = command.CabinClass,
            Passengers = command.Passengers,
            IsRoundTrip = command.IsRoundTrip,
            MilesUsed = command.MilesUsed,
            Taxes = command.Taxes,
            CashPrice = command.CashPrice
        };
        var result = await Sender.Send(created, cancellationToken);
        return Convert(result);
    }

    /// <summary>
    /// Updates an existing flight redemption.
    /// </summary>
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<Guid>> Update(Guid id, [FromBody] UpdateFlightRedemptionCommand command, CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        var updated = new UpdateFlightRedemptionCommand
        {
            Id = id,
            UserId = userId,
            ProgramId = command.ProgramId,
            OriginAirportCode = command.OriginAirportCode,
            DestinationAirportCode = command.DestinationAirportCode,
            FlightDate = command.FlightDate,
            CabinClass = command.CabinClass,
            Passengers = command.Passengers,
            IsRoundTrip = command.IsRoundTrip,
            MilesUsed = command.MilesUsed,
            Taxes = command.Taxes,
            CashPrice = command.CashPrice
        };
        var result = await Sender.Send(updated, cancellationToken);
        return Convert(result);
    }

    /// <summary>
    /// Deletes a flight redemption.
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await Sender.Send(new DeleteFlightRedemptionCommand { Id = id }, cancellationToken);
        return Convert(result);
    }
}
