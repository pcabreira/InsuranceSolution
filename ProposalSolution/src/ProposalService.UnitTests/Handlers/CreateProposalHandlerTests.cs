using Moq;
using ProposalService.Application.Commands;
using ProposalService.Application.Handlers;
using ProposalService.Domain.Entities;
using ProposalService.Domain.Ports;

namespace ProposalService.Application.Tests
{
    public class CreateProposalHandlerTests
    {
        [Fact]
        public async Task Handle_ShouldCreateProposal_AndReturnId()
        {
            // Arrange
            var repoMock = new Mock<IProposalRepository>();

            Proposal? addedProposal = null;

            repoMock.Setup(r => r.AddAsync(It.IsAny<Proposal>(), It.IsAny<CancellationToken>()))
                    .Returns(Task.CompletedTask)
                    .Callback<Proposal, CancellationToken>((p, _) => addedProposal = p);

            repoMock.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
                    .Returns(Task.CompletedTask);

            var handler = new CreateProposalHandler(repoMock.Object);

            var command = new CreateProposalCommand("Nova proposta", 1000m);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            repoMock.Verify(r => r.AddAsync(It.IsAny<Proposal>(), It.IsAny<CancellationToken>()), Times.Once);
            repoMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

            Assert.NotEqual(Guid.Empty, result);
            Assert.NotNull(addedProposal);
            Assert.Equal(result, addedProposal!.Id);
            Assert.Equal("Nova proposta", addedProposal.Title);
            Assert.Equal(1000m, addedProposal.Amount);
        }

        [Fact]
        public async Task Handle_WhenRepositoryThrowsException_ShouldThrow()
        {
            // Arrange
            var repoMock = new Mock<IProposalRepository>();

            repoMock.Setup(r => r.AddAsync(It.IsAny<Proposal>(), It.IsAny<CancellationToken>()))
                    .ThrowsAsync(new InvalidOperationException("Erro no repositório"));

            var handler = new CreateProposalHandler(repoMock.Object);
            var command = new CreateProposalCommand("Nova proposta", 1000m);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                handler.Handle(command, CancellationToken.None));

            Assert.Equal("Erro no repositório", exception.Message);
        }

        [Fact]
        public async Task Handle_ShouldCallAddAndSaveChanges_Once()
        {
            // Arrange
            var repoMock = new Mock<IProposalRepository>();

            repoMock.Setup(r => r.AddAsync(It.IsAny<Proposal>(), It.IsAny<CancellationToken>()))
                    .Returns(Task.CompletedTask);

            repoMock.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
                    .Returns(Task.CompletedTask);

            var handler = new CreateProposalHandler(repoMock.Object);
            var command = new CreateProposalCommand("Nova proposta", 1000m);

            // Act
            await handler.Handle(command, CancellationToken.None);

            // Assert
            repoMock.Verify(r => r.AddAsync(It.IsAny<Proposal>(), It.IsAny<CancellationToken>()), Times.Once);
            repoMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
