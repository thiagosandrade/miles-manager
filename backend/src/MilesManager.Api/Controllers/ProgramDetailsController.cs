using MediatR;
using Microsoft.AspNetCore.Mvc;
using MilesManager.Api.Common;
using MilesManager.Application.Features.ProgramDetails.DTOs;
using MilesManager.Application.Features.ProgramDetails.Queries.GetProgramDetails;

namespace MilesManager.Api.Controllers;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/program-details")]
public sealed class ProgramDetailsController(ISender sender) : ApiController(sender)
{
    [HttpGet("{programId:guid}")]
    public async Task<ActionResult<ProgramDetailsDto>> Get(Guid programId, CancellationToken cancellationToken)
    {
        var result = await Sender.Send(
            new GetProgramDetailsQuery(GetUserId(), programId),
            cancellationToken);

        return Convert(result);
    }
}
