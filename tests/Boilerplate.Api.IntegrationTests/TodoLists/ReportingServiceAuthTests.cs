using System.Net;
using Shouldly;
using Xunit;

namespace Boilerplate.Api.IntegrationTests.TodoLists;

[Collection(ApiCollection.Name)]
public sealed class ReportingServiceAuthTests(ApiFactory apiFactory)
{
    [Fact]
    public async Task Get_WrongClientId_ReturnsUnauthorized()
    {
        // Arrange
        using var client = apiFactory.CreateClient();
        client.DefaultRequestHeaders.Add("X-Client-Id", "wrong");

        // Act
        var response = await client.GetAsync(
            "/api/integration/reporting-service/todo-lists",
            TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Get_CorrectClientId_ReturnsOk()
    {
        // Arrange
        using var client = apiFactory.CreateClient();
        client.DefaultRequestHeaders.Add("X-Client-Id", "reporting-service");

        // Act
        var response = await client.GetAsync(
            "/api/integration/reporting-service/todo-lists",
            TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }
}
