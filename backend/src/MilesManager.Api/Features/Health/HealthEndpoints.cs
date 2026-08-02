namespace MilesManager.Api.Features.Health;

/// <summary>
/// HealthEndpoints.
/// </summary>
public static class HealthEndpoints
{
    /// <summary>
    /// Maps the API health check endpoint.
    /// </summary>
    public static IEndpointRouteBuilder MapHealthEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/health", () => Results.Ok(new { status = "ok" }))
            .WithName("HealthCheck")
            .WithTags("Health")
            .WithSummary("Returns service health.")
            .Produces(StatusCodes.Status200OK);

        return app;
    }
}
