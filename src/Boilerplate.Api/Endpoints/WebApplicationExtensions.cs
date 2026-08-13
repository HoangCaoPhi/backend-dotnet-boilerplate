using System.Reflection;
using Boilerplate.Api.Common;

namespace Boilerplate.Api.Endpoints;

public static class WebApplicationExtensions
{
    public static void MapEndpoints(
        this WebApplication app,
        Assembly assembly)
    {
        var group = app
            .MapGroup(string.Empty)
            .AddEndpointFilter<ResultEndpointFilter>();

        var endpointGroupTypes = assembly.GetTypes()
            .Where(type => type.IsClass && !type.IsAbstract && typeof(IEndpointGroup).IsAssignableFrom(type));

        foreach (var endpointGroupType in endpointGroupTypes)
        {
            var mapMethod = endpointGroupType.GetMethod(
                nameof(IEndpointGroup.Map),
                BindingFlags.Public | BindingFlags.Static)!;

            mapMethod.Invoke(
                null,
                [group]);
        }
    }
}
