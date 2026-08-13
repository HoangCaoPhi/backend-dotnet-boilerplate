using System.Security.Cryptography;
using System.Text;

namespace Boilerplate.Api.Endpoints.Integration;

// X-Client-Id always required; if ClientSecret is configured, also X-Token =
// HMAC-SHA256(clientSecret, X-Timestamp) within the replay window.
public sealed class IntegrationAuthFilter(string callerName) : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(
        EndpointFilterInvocationContext context,
        EndpointFilterDelegate next)
    {
        var configuration = context.HttpContext.RequestServices.GetRequiredService<IConfiguration>();
        var headers = context.HttpContext.Request.Headers;

        var expectedClientId = configuration[$"Integration:{callerName}:ClientId"];

        if (!headers.TryGetValue(
                "X-Client-Id",
                out var providedClientId)
            || providedClientId != expectedClientId)
        {
            return Results.Unauthorized();
        }

        var clientSecret = configuration[$"Integration:{callerName}:ClientSecret"];

        if (clientSecret is not null
            && !IsValidProofKeyToken(
                clientSecret,
                headers,
                configuration))
        {
            return Results.Unauthorized();
        }

        return await next(context);
    }

    private static bool IsValidProofKeyToken(
        string clientSecret,
        IHeaderDictionary headers,
        IConfiguration configuration)
    {
        if (!headers.TryGetValue(
                "X-Timestamp",
                out var timestampHeader)
            || !long.TryParse(
                timestampHeader,
                out var timestampSeconds)
            || !IsWithinReplayWindow(
                timestampSeconds,
                configuration))
        {
            return false;
        }

        return headers.TryGetValue(
                "X-Token",
                out var providedToken)
            && IsValidToken(
                clientSecret,
                timestampHeader.ToString(),
                providedToken.ToString());
    }

    private static bool IsWithinReplayWindow(
        long timestampSeconds,
        IConfiguration configuration)
    {
        var replayWindowMinutes = configuration.GetValue(
            "Integration:ReplayWindowMinutes",
            5);

        var requestTime = DateTimeOffset.FromUnixTimeSeconds(timestampSeconds);

        return (DateTimeOffset.UtcNow - requestTime).Duration() <= TimeSpan.FromMinutes(replayWindowMinutes);
    }

    private static bool IsValidToken(
        string clientSecret,
        string timestamp,
        string providedTokenHex)
    {
        byte[] providedBytes;

        try
        {
            providedBytes = Convert.FromHexString(providedTokenHex);
        }
        catch (FormatException)
        {
            return false;
        }

        var expectedBytes = HMACSHA256.HashData(
            Encoding.UTF8.GetBytes(clientSecret),
            Encoding.UTF8.GetBytes(timestamp));

        return CryptographicOperations.FixedTimeEquals(
            expectedBytes,
            providedBytes);
    }
}
