namespace ProposalService.Application.Ports;

public interface IEventPublisher
{
    Task PublishAsync(string routingKey, object message, CancellationToken cancellationToken = default);
}
