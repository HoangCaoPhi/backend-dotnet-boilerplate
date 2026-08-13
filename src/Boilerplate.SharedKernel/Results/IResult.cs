namespace Boilerplate.SharedKernel.Results;

public interface IResult<TSelf> where TSelf : IResult<TSelf>
{
    static abstract TSelf Failure(Error error);
}
