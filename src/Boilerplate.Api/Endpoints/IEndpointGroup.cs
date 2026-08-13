namespace Boilerplate.Api.Endpoints;

public interface IEndpointGroup
{
    static abstract void Map(IEndpointRouteBuilder app);
}
