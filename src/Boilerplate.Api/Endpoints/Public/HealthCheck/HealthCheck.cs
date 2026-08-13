namespace Boilerplate.Api.Endpoints.Public.HealthCheck;

public sealed record HealthCheckResponse(string Status);

public sealed class HealthCheck : IPublicEndpointGroup
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapGet(
            "/health",
            Handle);

    private static IResult Handle()
        => Results.Ok(new HealthCheckResponse("Healthy"));
}
