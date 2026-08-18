using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Boilerplate.Api.Common;
using Boilerplate.Application.TodoLists.Queries.GetTodoLists;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace Boilerplate.Api.IntegrationTests.TodoLists;

[Collection(ApiCollection.Name)]
public sealed class CreateTodoListTests(ApiFactory apiFactory)
{
    [Fact]
    public async Task Post_NoAuthorizationHeader_ReturnsUnauthorized()
    {
        // Arrange
        using var client = apiFactory.CreateClient();

        // Act
        var response = await client.PostAsJsonAsync(
            "/api/todo-lists",
            new { title = "Groceries" },
            TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Post_MissingIdempotencyKey_ReturnsBadRequest()
    {
        // Arrange
        using var client = CreateAuthorizedClient();

        // Act
        var response = await client.PostAsJsonAsync(
            "/api/todo-lists",
            new { title = "Groceries" },
            TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Post_ValidRequest_CreatesTodoListVisibleInGetTodoLists()
    {
        // Arrange
        using var client = CreateAuthorizedClient();
        client.DefaultRequestHeaders.Add(
            "Idempotency-Key",
            Guid.CreateVersion7().ToString());

        // Act
        var createResponse = await client.PostAsJsonAsync(
            "/api/todo-lists",
            new { title = "Groceries", colourCode = (string?)null },
            TestContext.Current.CancellationToken);
        createResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
        var createdId = await createResponse.Content.ReadFromJsonAsync<Guid>(TestContext.Current.CancellationToken);

        var lists = await client.GetFromJsonAsync<List<TodoListBriefDto>>(
            "/api/todo-lists",
            TestContext.Current.CancellationToken);

        // Assert
        lists.ShouldNotBeNull();
        lists.ShouldContain(list => list.Id == createdId && list.Title == "Groceries");
    }

    [Fact]
    public async Task Post_DuplicateIdempotencyKey_ReturnsConflict()
    {
        // Arrange
        using var client = CreateAuthorizedClient();
        client.DefaultRequestHeaders.Add(
            "Idempotency-Key",
            Guid.CreateVersion7().ToString());
        var request = new { title = "Groceries", colourCode = (string?)null };

        await client.PostAsJsonAsync(
            "/api/todo-lists",
            request,
            TestContext.Current.CancellationToken);

        // Act
        var secondResponse = await client.PostAsJsonAsync(
            "/api/todo-lists",
            request,
            TestContext.Current.CancellationToken);

        // Assert
        secondResponse.StatusCode.ShouldBe(HttpStatusCode.Conflict);
    }

    private HttpClient CreateAuthorizedClient()
    {
        var client = apiFactory.CreateClient();
        var configuration = apiFactory.Services.GetRequiredService<IConfiguration>();
        var token = JwtTokenFactory.CreateToken(
            configuration,
            Guid.CreateVersion7());

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer",
            token);

        return client;
    }
}
