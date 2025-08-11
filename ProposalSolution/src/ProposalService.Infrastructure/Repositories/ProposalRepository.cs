using Microsoft.EntityFrameworkCore;
using ProposalService.Domain.Entities;
using ProposalService.Domain.Ports;
using ProposalService.Infrastructure.Data;

namespace ProposalService.Infrastructure.Repositories
{
    public class ProposalRepository : IProposalRepository
    {
        private readonly ProposalDbContext _ctx;

        public ProposalRepository(ProposalDbContext ctx) => _ctx = ctx;

        public Task AddAsync(Proposal proposal, CancellationToken cancellationToken = default) =>
            _ctx.AddAsync(proposal, cancellationToken).AsTask();

        public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
            _ctx.SaveChangesAsync(cancellationToken);

        public async Task<Proposal?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _ctx.Proposals
                .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        }

        public async Task UpdateAsync(Proposal proposal, CancellationToken cancellationToken = default)
        {
            _ctx.Proposals.Update(proposal);
            await _ctx.SaveChangesAsync(cancellationToken);
        }
    }
}

