using System.Net;
using Shouldly;
using Xunit;

namespace Boilerplate.Api.IntegrationTests;

[Collection(ApiCollection.Name)]
public sealed class HealthCheckTests(ApiFactory apiFactory)
{
    [Fact]
    public async Task Get_Health_ReturnsOk()
    {
        // Arrange
        using var client = apiFactory.CreateClient();

        // Act
        var response = await client.GetAsync(
            "/health",
            TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }
}
