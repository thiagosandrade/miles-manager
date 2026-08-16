using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MilesManager.Api.Common;
using MilesManager.Api.Contracts.Users;
using MilesManager.Application.Common;
using MilesManager.Application.Features.Users.Commands.CreateUser;
using MilesManager.Application.Features.Users.Commands.DeleteUser;
using MilesManager.Application.Features.Users.Commands.UpdateUser;
using MilesManager.Application.Features.Users.DTOs;
using MilesManager.Application.Features.Users.Queries.GetUserById;
using MilesManager.Application.Features.Users.Queries.GetUsers;

namespace MilesManager.Api.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/users")]
[ApiVersion("1.0")]
public sealed class UsersController(ISender sender) : ApiController(sender)
{
    [HttpPost]
    [Authorize(Roles = "Administrator")]
    public async Task<ActionResult<UserDetailsDTO>> Create([FromBody] CreateUserRequest request)
    {
        var result = await Sender.Send(new CreateUserCommand(request.Email, request.FullName, request.Password));
        return result.Status switch
        {
            ResultStatus.Success => Created($"/api/users/{result.Value!.Id}", result.Value),
            ResultStatus.Conflict => Conflict(),
            _ => BadRequest()
        };
    }

    [HttpGet]
    [Authorize(Roles = "Administrator")]
    public async Task<ActionResult<IReadOnlyList<UserDTO>>> GetUsers()
    {
        var result = await Sender.Send(new GetUsersQuery());
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [Authorize(Roles = "User")]
    public async Task<ActionResult<UserDetailsDTO>> GetById(Guid id)
    {
        var user = await Sender.Send(new GetUserByIdQuery(id));
        return user is null ? NotFound() : Ok(user);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Administrator")]
    public async Task<ActionResult<UserDetailsDTO>> Update(Guid id, [FromBody] UpdateUserRequest request)
    {
        var result = await Sender.Send(new UpdateUserCommand(id, request.Email, request.FullName, request.Password, request.IsActive));
        return result.Status switch
        {
            ResultStatus.Success => Ok(result.Value),
            ResultStatus.NotFound => NotFound(),
            ResultStatus.Conflict => Conflict(),
            _ => BadRequest()
        };
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await Sender.Send(new DeleteUserCommand(id));
        return result == OperationStatus.Success ? NoContent() : NotFound();
    }
}
