using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MilesManager.Api.Common;
using MilesManager.Application.Features.UserRoles.Commands.AssignRole;
using MilesManager.Application.Features.UserRoles.Commands.RemoveRole;
using MilesManager.Application.Features.UserRoles.DTOs;
using MilesManager.Application.Features.UserRoles.Queries.GetRoles;
using MilesManager.Application.Features.Users.DTOs;

namespace MilesManager.Api.Controllers;

[ApiController]
[Authorize(Roles = "Administrator")]
[Route("api/v{version:apiVersion}/user-roles")]
[ApiVersion("1.0")]
public sealed class UserRolesController(ISender sender) : ApiController(sender)
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<RoleDTO>>> GetRoles()
    {
        var result = await Sender.Send(new GetRolesQuery());
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> AssignRole([FromBody] AssignRoleCommand command)
    {
        var result = await Sender.Send(command);

        return result == OperationStatus.Success ? NoContent() : NotFound();
    }

    [HttpDelete]
    public async Task<IActionResult> RemoveRole([FromBody] RemoveRoleCommand command)
    {
        var result = await Sender.Send(command);

        return result == OperationStatus.Success ? NoContent() : NotFound();
    }
}
