using MediatR;
using Microsoft.AspNetCore.Mvc;
using MilesManager.Api.Common;
using MilesManager.Application.Features.ProgramBalances.DTOs;
using MilesManager.Application.Features.ProgramBalances.Queries.GetUserProgramPurchases;

namespace MilesManager.Api.Controllers;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/program-balances")]
public sealed class ProgramBalancesController(ISender sender) : ApiController(sender)
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<UserProgramBalancesDto>>> Get(
        CancellationToken cancellationToken)
    {
        var result = await Sender.Send(new GetUserProgramBalancesQuery(GetUserId()), cancellationToken);
        return Ok(result.Value);
    }
}
