using ContractService.Application.Commands;
using ContractService.Application.Handlers;
using ContractService.Application.Ports;
using ContractService.Domain.Entities;
using Moq;

namespace ContractService.Application.Tests
{
    public class CreateContractCommandHandlerTests
    {
        [Fact]
        public async Task Handle_ShouldCreateContract_AndReturnId()
        {
            // Arrange
            var repoMock = new Mock<IContractRepository>();

            // Simula que não existe contrato para essa proposta
            repoMock.Setup(r => r.GetByProposalIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync((Contract?)null);

            Contract? addedContract = null;
            repoMock.Setup(r => r.AddAsync(It.IsAny<Contract>(), It.IsAny<CancellationToken>()))
                    .Returns(Task.CompletedTask)
                    .Callback<Contract, CancellationToken>((c, _) => addedContract = c);

            var handler = new CreateContractCommandHandler(repoMock.Object);

            var proposalId = Guid.NewGuid();
            var contractDate = DateTime.UtcNow;
            var command = new CreateContractCommand(proposalId, contractDate);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            repoMock.Verify(r => r.GetByProposalIdAsync(proposalId, It.IsAny<CancellationToken>()), Times.Once);
            repoMock.Verify(r => r.AddAsync(It.IsAny<Contract>(), It.IsAny<CancellationToken>()), Times.Once);

            Assert.NotEqual(Guid.Empty, result);
            Assert.NotNull(addedContract);
            Assert.Equal(result, addedContract!.Id);
            Assert.Equal(proposalId, addedContract.ProposalId);
            Assert.Equal(contractDate, addedContract.ContractDate);
        }

        [Fact]
        public async Task Handle_WhenContractAlreadyExists_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var repoMock = new Mock<IContractRepository>();

            var proposalId = Guid.NewGuid();
            var fakeContract = Contract.Create(proposalId, DateTime.UtcNow);

            // Simula que já existe contrato para a proposta
            repoMock.Setup(r => r.GetByProposalIdAsync(proposalId, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(fakeContract);

            var handler = new CreateContractCommandHandler(repoMock.Object);

            var command = new CreateContractCommand(proposalId, DateTime.UtcNow);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                handler.Handle(command, CancellationToken.None));

            Assert.Equal($"Contrato já existe para a proposta {proposalId}", exception.Message);

            repoMock.Verify(r => r.GetByProposalIdAsync(proposalId, It.IsAny<CancellationToken>()), Times.Once);
            repoMock.Verify(r => r.AddAsync(It.IsAny<Contract>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldCallGetByProposalIdAsync_AndAddAsync_Once()
        {
            // Arrange
            var repoMock = new Mock<IContractRepository>();

            repoMock.Setup(r => r.GetByProposalIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync((Contract?)null);

            repoMock.Setup(r => r.AddAsync(It.IsAny<Contract>(), It.IsAny<CancellationToken>()))
                    .Returns(Task.CompletedTask);

            var handler = new CreateContractCommandHandler(repoMock.Object);
            var command = new CreateContractCommand(Guid.NewGuid(), DateTime.UtcNow);

            // Act
            await handler.Handle(command, CancellationToken.None);

            // Assert
            repoMock.Verify(r => r.GetByProposalIdAsync(command.ProposalId, It.IsAny<CancellationToken>()), Times.Once);
            repoMock.Verify(r => r.AddAsync(It.IsAny<Contract>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}

