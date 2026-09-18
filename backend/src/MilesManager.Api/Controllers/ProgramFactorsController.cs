using MediatR;
using Microsoft.AspNetCore.Mvc;
using MilesManager.Api.Common;
using MilesManager.Application.Features.ProgramFactors.Commands.CreateProgramFactor;
using MilesManager.Application.Features.ProgramFactors.Commands.DeleteProgramFactor;
using MilesManager.Application.Features.ProgramFactors.Commands.UpdateProgramFactor;
using MilesManager.Application.Features.ProgramFactors.DTOs;
using MilesManager.Application.Features.ProgramFactors.Queries.GetAllProgramFactors;

namespace MilesManager.Api.Controllers;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/program-factors")]
public sealed class ProgramFactorsController(ISender sender) : ApiController(sender)
{
    [HttpPost]
    public async Task<ActionResult<Guid>> Create([FromBody] CreateProgramFactorCommand command, CancellationToken cancellationToken)
    {
        var result = await Sender.Send(command, cancellationToken);
        return Convert(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<Guid>> Update(Guid id, [FromBody] UpdateProgramFactorCommand command, CancellationToken cancellationToken)
    {
        command = new UpdateProgramFactorCommand
        {
            Id = id, OriginProgramId = command.OriginProgramId, DestinationProgramId = command.DestinationProgramId,
            FromAmount = command.FromAmount, ToAmount = command.ToAmount
        };
        var result = await Sender.Send(command, cancellationToken);
        return Convert(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await Sender.Send(new DeleteProgramFactorCommand { Id = id }, cancellationToken);
        return Convert(result);
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ProgramFactorDto>>> GetAll(
        CancellationToken cancellationToken)
    {
        var result = await Sender.Send(new GetAllProgramFactorsQuery(), cancellationToken);
        return Convert(result);
    }
}
