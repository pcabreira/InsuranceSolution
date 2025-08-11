using Microsoft.Extensions.Options;
using RabbitMQ.Client;

public class RabbitMqSetup
{
    private readonly RabbitMqSettings _settings;
    private readonly IConnection _connection;
    private readonly IModel _channel;

    public RabbitMqSetup(IOptions<RabbitMqSettings> options)
    {
        _settings = options.Value;

        var factory = new ConnectionFactory()
        {
            HostName = _settings.Host,
            Port = _settings.Port,
            UserName = _settings.Username,
            Password = _settings.Password,
            DispatchConsumersAsync = true
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        SetupExchangeAndQueues();
    }

    private void SetupExchangeAndQueues()
    {
        _channel.ExchangeDeclare(_settings.Exchange, ExchangeType.Direct, durable: true);

        foreach (var queue in _settings.Queues.Values)
        {
            _channel.QueueDeclare(queue.Name, durable: true, exclusive: false, autoDelete: false, arguments: null);
            _channel.QueueBind(queue.Name, _settings.Exchange, queue.RoutingKey);
        }
    }
}
