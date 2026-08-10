using MediatR;
using Microsoft.AspNetCore.Mvc;
using MilesManager.Api.Common;
using MilesManager.Application.Features.Programs.Commands.CreateProgram;
using MilesManager.Application.Features.Programs.Commands.UpdateProgram;
using MilesManager.Application.Features.Programs.Commands.DeleteProgram;
using MilesManager.Application.Features.Programs.Queries.GetPrograms;
using MilesManager.Application.Features.Programs.Queries.GetProgramById;

namespace MilesManager.Api.Controllers;

/// <summary>
/// Controller for managing loyalty programs.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="ProgramsController"/> class.
/// </remarks>
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/programs")]
public sealed class ProgramsController(ISender sender) : ApiController(sender)
{

    /// <summary>
    /// Gets all programs.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<System.Collections.Generic.IReadOnlyList<MilesManager.Application.Features.Programs.DTOs.ProgramDto>>> Get(CancellationToken cancellationToken)
    {
        var result = await Sender.Send(new GetProgramsQuery(), cancellationToken);
        return Convert(result);
    }

    /// <summary>
    /// Gets a program by id.
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<MilesManager.Application.Features.Programs.DTOs.ProgramDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await Sender.Send(new GetProgramByIdQuery { Id = id }, cancellationToken);
        return Convert(result);
    }

    /// <summary>
    /// Creates a new program.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<Guid>> Create([FromBody] CreateProgramCommand command, CancellationToken cancellationToken)
    {
        var result = await Sender.Send(command, cancellationToken);
        return Convert(result);
    }

    /// <summary>
    /// Updates an existing program.
    /// </summary>
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<Guid>> Update(Guid id, [FromBody] UpdateProgramCommand command, CancellationToken cancellationToken)
    {
        var updated = new UpdateProgramCommand
        {
            Id = id,
            ProgramCode = command.ProgramCode,
            Name = command.Name,
            ProgramType = command.ProgramType,
            Website = command.Website,
            Logo = command.Logo,
            ReferencePricePerThousand = command.ReferencePricePerThousand,
            IsActive = command.IsActive
        };

        var result = await Sender.Send(updated, cancellationToken);
        return Convert(result);
    }

    /// <summary>
    /// Deletes a program.
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await Sender.Send(new DeleteProgramCommand { Id = id }, cancellationToken);
        return Convert(result);
    }
}
