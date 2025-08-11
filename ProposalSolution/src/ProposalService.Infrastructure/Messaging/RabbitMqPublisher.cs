using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using ProposalService.Application.Ports;
using RabbitMQ.Client;
using System.Text;

namespace ProposalService.Infrastructure.Messaging
{
    public sealed class RabbitMQPublisher : IEventPublisher, IDisposable
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly string _exchange;

        public RabbitMQPublisher(IConfiguration configuration)
        {
            var section = configuration.GetSection("RabbitMq");
            var host = section["Host"];
            var port = int.Parse(section["Port"]);
            var username = section["Username"];
            var password = section["Password"];
            var queuesSection = section.GetSection("Queues");
            _exchange = section["Exchange"];

            var factory = new ConnectionFactory
            {
                HostName = host,
                Port = port,
                UserName = username,
                Password = password
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            // Declara exchange
            _channel.ExchangeDeclare(
                exchange: _exchange,
                type: ExchangeType.Direct,
                durable: true);

            // Declara todas as filas e bindings configurados na seção "Queues"
            foreach (var queueSection in queuesSection.GetChildren())
            {
                var queueName = queueSection["Name"];
                var routingKey = queueSection["RoutingKey"];

                _channel.QueueDeclare(
                    queue: queueName,
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

                _channel.QueueBind(
                    queue: queueName,
                    exchange: _exchange,
                    routingKey: routingKey);
            }
        }

        public Task PublishAsync(string routingKey, object message, CancellationToken cancellationToken = default)
        {
            var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));

            var props = _channel.CreateBasicProperties();
            props.Persistent = true;

            _channel.BasicPublish(exchange: _exchange, routingKey: routingKey, basicProperties: props, body: body);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _channel?.Close();
            _channel?.Dispose();

            _connection?.Close();
            _connection?.Dispose();
        }
    }
}


