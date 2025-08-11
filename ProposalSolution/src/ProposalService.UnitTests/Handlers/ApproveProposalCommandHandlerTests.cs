using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using ProposalService.Application.Commands;
using ProposalService.Application.Ports;
using ProposalService.Domain.Entities;
using ProposalService.Domain.Enums;
using ProposalService.Domain.Ports;

public class ApproveProposalCommandHandlerTests
{
    [Fact]
    public async Task Handle_ShouldReturnFalse_WhenProposalNotFound()
    {
        // Arrange
        var repoMock = new Mock<IProposalRepository>();
        var publisherMock = new Mock<IEventPublisher>();

        repoMock
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Proposal?)null);

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    { "RabbitMQ:HostName", "localhost" },
                    { "RabbitMQ:QueueName", "proposals.approved" },
                    { "RabbitMQ:AutoAck", "true" }
                })
                .Build();

        var handler = new ApproveProposalCommandHandler(repoMock.Object, publisherMock.Object, configuration);
        var command = new ApproveProposalCommand(Guid.NewGuid());

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeFalse();
        repoMock.Verify(r => r.UpdateAsync(It.IsAny<Proposal>(), It.IsAny<CancellationToken>()), Times.Never);
        publisherMock.Verify(p => p.PublishAsync(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldApproveProposal_UpdateRepository_AndPublishEvent()
    {
        // Arrange
        var proposalId = Guid.NewGuid();
        var existingProposal = Proposal.Create("Título de teste", 1000m);

        // Ajusta o Id para o Guid desejado, pois Create gera um Id novo
        typeof(Proposal).GetProperty(nameof(Proposal.Id))!.SetValue(existingProposal, proposalId);

        var repoMock = new Mock<IProposalRepository>();
        var publisherMock = new Mock<IEventPublisher>();

        // Configuração real com todas as chaves necessárias, inclusive routing key
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
            { "RabbitMQ:HostName", "localhost" },
            { "RabbitMQ:QueueName", "proposals.approved" },
            { "RabbitMQ:AutoAck", "true" },
            { "RabbitMQ:Queues:StatusApproved:RoutingKey", "proposal.approved" }
            })
            .Build();

        repoMock
            .Setup(r => r.GetByIdAsync(proposalId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingProposal);

        repoMock
            .Setup(r => r.UpdateAsync(It.IsAny<Proposal>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        publisherMock
            .Setup(p => p.PublishAsync(
                "proposal.approved",
                It.Is<object>(o => IsExpectedProposalMessage(o, proposalId)),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var handler = new ApproveProposalCommandHandler(repoMock.Object, publisherMock.Object, configuration);
        var command = new ApproveProposalCommand(proposalId);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
        existingProposal.Status.Should().Be(ProposalStatus.Approved);

        repoMock.Verify(r => r.UpdateAsync(It.Is<Proposal>(p => p == existingProposal), It.IsAny<CancellationToken>()), Times.Once);
        publisherMock.Verify(p => p.PublishAsync("proposal.approved", It.IsAny<object>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    private bool IsExpectedProposalMessage(object o, Guid proposalId)
    {
        var idProp = o.GetType().GetProperty(nameof(Proposal.Id));
        var statusProp = o.GetType().GetProperty(nameof(Proposal.Status));

        if (idProp == null || statusProp == null)
            return false;

        var idValue = idProp.GetValue(o);
        var statusValue = statusProp.GetValue(o);

        if (idValue == null || statusValue == null)
            return false;

        // Sem pattern matching, use casts
        if (!(idValue is Guid guid)) return false;
        if (!(statusValue is string statusStr)) return false;

        return guid == proposalId && statusStr == ProposalStatus.Approved.ToString();
    }

}


