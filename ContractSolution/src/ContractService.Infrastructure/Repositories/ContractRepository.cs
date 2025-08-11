using ContractService.Application.Ports;
using ContractService.Domain.Entities;
using ContractService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ContractService.Infrastructure.Repositories
{
    public class ContractRepository : IContractRepository
    {
        private readonly ContractDbContext _ctx;

        public ContractRepository(ContractDbContext ctx) => _ctx = ctx;

        public async Task AddAsync(Contract contract, CancellationToken cancellationToken = default)
        {
            await _ctx.Contracts.AddAsync(contract, cancellationToken);
            await _ctx.SaveChangesAsync(cancellationToken);
        }

        public async Task<Contract?> GetByProposalIdAsync(Guid proposalId, CancellationToken cancellationToken = default)
        {
            return await _ctx.Contracts
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.ProposalId == proposalId, cancellationToken);
        }
    }
}
