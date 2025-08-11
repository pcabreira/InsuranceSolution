using Microsoft.EntityFrameworkCore;
using ProposalService.Domain.Entities;
using ProposalService.Infrastructure.Data;
using ProposalService.Infrastructure.Repositories;

public class ProposalRepositoryIntegrationTests : IDisposable
{
    private readonly ProposalDbContext _context;

    public ProposalRepositoryIntegrationTests()
    {
        var options = new DbContextOptionsBuilder<ProposalDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb")
            .Options;

        _context = new ProposalDbContext(options);
    }

    [Fact]
    public async Task AddAsync_ShouldAddProposal()
    {
        var repository = new ProposalRepository(_context);

        // Corrigido: parâmetros para Create conforme sua entidade (string title, decimal amount)
        var proposal = Proposal.Create("Título de Teste", 1000m);

        await repository.AddAsync(proposal, default);

        var savedProposal = await _context.Proposals.FindAsync(proposal.Id);

        Assert.NotNull(savedProposal);
        Assert.Equal(proposal.Id, savedProposal.Id);
        Assert.Equal(proposal.Title, savedProposal.Title);
        Assert.Equal(proposal.Amount, savedProposal.Amount);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
