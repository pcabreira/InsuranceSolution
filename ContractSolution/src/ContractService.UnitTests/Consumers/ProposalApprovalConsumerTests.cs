using ContractService.Application.Commands;
using ContractService.Application.Consumers;
using ContractService.Application.Events;
using ContractService.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using System.Text.Json;

namespace ContractService.UnitTests.Consumers
{
    public class ProposalApprovalConsumerTests
    {
        [Fact]
        public async Task Received_ShouldSendCreateContractCommand_WhenStatusApproved()
        {
            // Arrange
            var mediatorMock = new Mock<IMediator>();
            mediatorMock
                .Setup(m => m.Send(It.IsAny<CreateContractCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Guid.NewGuid());

            var serviceScopeMock = new Mock<IServiceScope>();
            serviceScopeMock
                .Setup(s => s.ServiceProvider.GetService(typeof(IMediator)))
                .Returns(mediatorMock.Object);

            var scopeFactoryMock = new Mock<IServiceScopeFactory>();
            scopeFactoryMock
                .Setup(sf => sf.CreateScope())
                .Returns(serviceScopeMock.Object);

            var loggerMock = new Mock<ILogger<ProposalApprovalConsumer>>();

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    { "RabbitMQ:HostName", "localhost" },
                    { "RabbitMQ:QueueName", "proposals.approved" },
                    { "RabbitMQ:AutoAck", "true" }
                })
                .Build();

            var consumer = new ProposalApprovalConsumer(scopeFactoryMock.Object, loggerMock.Object, configuration);

            var proposalEvent = new ProposalApprovedEvent(Guid.NewGuid(), ProposalStatus.Approved.ToString());
            var jsonMessage = JsonSerializer.Serialize(proposalEvent);

            // Act
            await consumer.ProcessMessageAsync(jsonMessage, true, 1UL);

            // Assert
            mediatorMock.Verify(
                m => m.Send(It.Is<CreateContractCommand>(cmd => cmd.ProposalId == proposalEvent.ProposalId),
                It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}


