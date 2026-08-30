using MediatR;
using Microsoft.AspNetCore.Mvc;
using MilesManager.Api.Common;
using MilesManager.Application.Features.ProgramFactors.Commands.CreateProgramFactor;
using MilesManager.Application.Features.ProgramFactors.Commands.UpdateProgramFactor;
using MilesManager.Application.Features.ProgramFactors.Commands.DeleteProgramFactor;
using MilesManager.Application.Features.ProgramFactors.Queries.GetAllProgramFactors;

namespace MilesManager.Api.Controllers;

/// <summary>
/// Controller for program transfer factors.
/// </summary>
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/program-factors")]
public sealed class ProgramFactorsController(ISender sender) : ApiController(sender)
{
    /// <summary>
    /// Creates a transfer factor between two mileage programs.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<Guid>> Create(
        [FromBody] CreateProgramFactorCommand command,
        CancellationToken cancellationToken)
    {
        var result = await Sender.Send(command, cancellationToken);
        return Convert(result);
    }

    /// <summary>
    /// Updates a transfer factor between two mileage programs.
    /// </summary>
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<Guid>> Update(
        Guid id,
        [FromBody] UpdateProgramFactorCommand command,
        CancellationToken cancellationToken)
    {
        command = new UpdateProgramFactorCommand
        {
            Id = id,
            OriginProgramId = command.OriginProgramId,
            DestinationProgramId = command.DestinationProgramId,
            TransferFactor = command.TransferFactor
        };

        var result = await Sender.Send(command, cancellationToken);
        return Convert(result);
    }

    /// <summary>
    /// Deletes a transfer factor between two mileage programs.
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await Sender.Send(new DeleteProgramFactorCommand { Id = id }, cancellationToken);
        return Convert(result);
    }

    /// <summary>
    /// Gets all transfer factors.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await Sender.Send(new GetAllProgramFactorsQuery(), cancellationToken);
        return Convert(result);
    }
}
