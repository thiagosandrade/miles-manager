using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MilesManager.Api.Common;

namespace MilesManager.Api.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/health")]
[ApiVersion("1.0")]
public sealed class HealthController(ISender sender) : ApiController(sender)
{
    [HttpGet]
    [AllowAnonymous]
    public ActionResult Get()
    {
        return Ok(new { status = "ok" });
    }
}
