using ContractService.Domain.Entities;
using ContractService.Infrastructure.Data;
using ContractService.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

public class ContractRepositoryIntegrationTests : IDisposable
{
    private readonly ContractDbContext _context;

    public ContractRepositoryIntegrationTests()
    {
        var options = new DbContextOptionsBuilder<ContractDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb")
            .Options;

        _context = new ContractDbContext(options);
    }

    [Fact]
    public async Task AddAsync_ShouldAddContract()
    {
        var repository = new ContractRepository(_context);

        // Método factory Create para instanciar
        var contract = Contract.Create(Guid.NewGuid(), DateTime.UtcNow);

        await repository.AddAsync(contract, default);

        var savedContract = await _context.Contracts.FindAsync(contract.Id);

        Assert.NotNull(savedContract);
        Assert.Equal(contract.ProposalId, savedContract.ProposalId);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
