namespace Boilerplate.Application.Common.ExternalClients.Slack;

public interface ISlackClient
{
    Task NotifyAsync(
        string message,
        CancellationToken cancellationToken);
}
