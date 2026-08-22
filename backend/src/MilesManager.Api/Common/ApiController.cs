using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using MilesManager.Application.Common;

namespace MilesManager.Api.Common;

/// <summary>
/// Base API controller that provides common helpers and access to MediatR sender.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="ApiController"/> class.
/// </remarks>
/// <param name="sender">The MediatR sender.</param>
[ApiController]
[Authorize]
public abstract class ApiController(ISender sender) : ControllerBase
{
    protected readonly ISender Sender = sender;

    /// <summary>
    /// Gets the current authenticated user's id from the claims principal.
    /// </summary>
    /// <returns>The user id or Guid.Empty when not available.</returns>
    protected Guid GetUserId()
    {
        var sub = User?.FindFirstValue("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");
        return Guid.TryParse(sub, out var id) ? id : Guid.Empty;
    }

    /// <summary>
    /// Converts a non-generic <see cref="Result"/> to an <see cref="ActionResult"/>.
    /// </summary>
    protected ActionResult Convert(Result result)
    {
        return result.Status switch
        {
            ResultStatus.Success => NoContent(),
            ResultStatus.NotFound => NotFound(),
            ResultStatus.ValidationFailed => BadRequest(new ProblemDetails { Title = result.Error ?? "Validation failed" }),
            ResultStatus.Conflict => Conflict(new ProblemDetails { Title = result.Error ?? "Conflict" }),
            ResultStatus.Unauthorized => Unauthorized(),
            _ => StatusCode(500)
        };
    }

    /// <summary>
    /// Converts a generic <see cref="Result{T}"/> to an <see cref="ActionResult{T}"/>.
    /// </summary>
    protected ActionResult<T> Convert<T>(Result<T> result)
    {
        return result.Status switch
        {
            ResultStatus.Success when result.Value is not null => Ok(result.Value),
            ResultStatus.Success => NoContent(),
            ResultStatus.NotFound => NotFound(),
            ResultStatus.ValidationFailed => BadRequest(new ValidationProblemDetails { Title = result.Error ?? "Validation failed" }),
            ResultStatus.Conflict => Conflict(new ProblemDetails { Title = result.Error ?? "Conflict" }),
            ResultStatus.Unauthorized => Unauthorized(),
            _ => StatusCode(500)
        };
    }
}
