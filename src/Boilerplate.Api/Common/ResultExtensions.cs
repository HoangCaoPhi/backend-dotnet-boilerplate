using Boilerplate.SharedKernel.Results;

namespace Boilerplate.Api.Common;

public static class ResultExtensions
{
    public static IResult ToHttpResult(this Result result)
        => result.IsSuccess
            ? Results.NoContent()
            : result.Error.ToProblem();

    public static IResult ToHttpResult<T>(this Result<T> result)
        => result.IsSuccess
            ? Results.Ok(result.Value)
            : result.Error.ToProblem();

    private static IResult ToProblem(this Error error)
    {
        var statusCode = error.Type switch
        {
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            _ => StatusCodes.Status400BadRequest,
        };

        return Results.Problem(
            detail: error.Message,
            statusCode: statusCode,
            title: error.Code);
    }
}
