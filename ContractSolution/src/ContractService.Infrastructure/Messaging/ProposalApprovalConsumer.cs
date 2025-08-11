using ContractService.Application.Commands;
using ContractService.Application.Events;
using ContractService.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace ContractService.Application.Consumers
{
    public class ProposalApprovalConsumer : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<ProposalApprovalConsumer> _logger;
        private readonly IConfiguration _configuration;
        private readonly IConnection? _connectionInjected;
        private readonly IModel? _channelInjected;

        private IConnection? _connection;
        private IModel? _channel;

        public ProposalApprovalConsumer(
            IServiceScopeFactory scopeFactory,
            ILogger<ProposalApprovalConsumer> logger,
            IConfiguration configuration,
            IConnection? connection = null,
            IModel? channel = null)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
            _configuration = configuration;
            _connectionInjected = connection;
            _channelInjected = channel;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _connection = _connectionInjected ?? CreateConnection();
            _channel = _channelInjected ?? _connection.CreateModel();

            var queueName = _configuration["RabbitMQ:QueueName"] ?? "proposals.approved";
            bool autoAck = bool.TryParse(_configuration["RabbitMQ:AutoAck"], out var ack) ? ack : false;

            _channel.QueueDeclare(queue: queueName,
                                 durable: true,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                await ProcessMessageAsync(message, autoAck, ea.DeliveryTag);
            };

            _channel.BasicConsume(queue: queueName, autoAck: autoAck, consumer: consumer);

            return Task.Delay(Timeout.Infinite, stoppingToken);
        }

        private IConnection CreateConnection()
        {
            var factory = new ConnectionFactory
            {
                HostName = _configuration["RabbitMQ:HostName"] ?? "localhost"
            };
            return factory.CreateConnection();
        }

        // Método público que usa scope para obter o mediator e chama o interno
        public async Task ProcessMessageAsync(string message, bool autoAck, ulong deliveryTag)
        {
            using var scope = _scopeFactory.CreateScope();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            await ProcessMessageAsync(message, autoAck, deliveryTag, mediator);
        }

        // Método interno para facilitar testes, recebe IMediator injetado diretamente
        internal async Task ProcessMessageAsync(string message, bool autoAck, ulong deliveryTag, IMediator mediator)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                _logger.LogWarning("Mensagem vazia recebida. Ignorando processamento.");
                if (!autoAck) _channel?.BasicAck(deliveryTag, false);
                return;
            }

            try
            {
                var proposalEvent = JsonSerializer.Deserialize<ProposalApprovedEvent>(message);

                if (proposalEvent is null)
                {
                    _logger.LogWarning("Mensagem inválida recebida. Ignorando.");
                    if (!autoAck) _channel?.BasicAck(deliveryTag, false);
                    return;
                }

                if (proposalEvent.Status != ProposalStatus.Approved.ToString())
                {
                    _logger.LogInformation($"Proposta {proposalEvent.ProposalId} não aprovada. Nenhum contrato criado.");
                    if (!autoAck) _channel?.BasicAck(deliveryTag, false);
                    return;
                }

                await mediator.Send(new CreateContractCommand(proposalEvent.ProposalId, DateTime.UtcNow));
                _logger.LogInformation($"Contrato criado para proposta {proposalEvent.ProposalId}");

                if (!autoAck) _channel?.BasicAck(deliveryTag, false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar mensagem do RabbitMQ");
                if (!autoAck) _channel?.BasicNack(deliveryTag, false, true);
            }
        }

        public override void Dispose()
        {
            _channel?.Close();
            _connection?.Close();
            base.Dispose();
        }
    }
}
