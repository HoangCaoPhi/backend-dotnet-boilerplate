using Boilerplate.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;
using Testcontainers.RabbitMq;
using Xunit;

namespace Boilerplate.Api.IntegrationTests;

public sealed class ApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder("postgres:18")
        .WithDatabase("boilerplate")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .Build();

    private readonly RabbitMqContainer _rabbitMq = new RabbitMqBuilder("rabbitmq:4-management")
        .Build();

    public Uri RabbitMqUri => new(_rabbitMq.GetConnectionString());

    protected override void ConfigureWebHost(IWebHostBuilder builder)
        => builder
            .UseSetting(
                "ConnectionStrings:Database",
                _postgres.GetConnectionString())
            .UseSetting(
                "RabbitMq:Host",
                RabbitMqUri.Host)
            .UseSetting(
                "RabbitMq:Port",
                RabbitMqUri.Port.ToString())
            .UseSetting(
                "RabbitMq:Username",
                RabbitMqUri.UserInfo.Split(':')[0])
            .UseSetting(
                "RabbitMq:Password",
                RabbitMqUri.UserInfo.Split(':')[1]);

    async ValueTask IAsyncLifetime.InitializeAsync()
    {
        await Task.WhenAll(
            _postgres.StartAsync(),
            _rabbitMq.StartAsync());

        using var scope = Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        await context.Database.EnsureCreatedAsync();
    }

    public override async ValueTask DisposeAsync()
    {
        await base.DisposeAsync();
        await Task.WhenAll(
            _postgres.DisposeAsync().AsTask(),
            _rabbitMq.DisposeAsync().AsTask());
    }
}
