using Boilerplate.SharedKernel.Results;

namespace Boilerplate.Api.Common;

public sealed class ResultEndpointFilter : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(
        EndpointFilterInvocationContext context,
        EndpointFilterDelegate next)
    {
        var response = await next(context);

        if (response is not Result result)
        {
            return response;
        }

        if (!result.IsSuccess)
        {
            return result.ToHttpResult();
        }

        var value = result.GetValue();

        return value is null
            ? Results.NoContent()
            : Results.Ok(value);
    }
}
