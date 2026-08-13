namespace Boilerplate.SharedKernel.Results;

public class Result : IResult<Result>
{
    public bool IsSuccess { get; }

    public Error Error { get; }

    protected Result(
        bool isSuccess,
        Error error)
    {
        IsSuccess = isSuccess;
        Error = error;
    }

    public static Result Success() => new(
        true,
        Error.None);

    public static Result Failure(Error error) => new(
        false,
        error);

    public virtual object? GetValue() => null;
}
