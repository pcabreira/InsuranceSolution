using ProposalService.Domain.Entities;

namespace ProposalService.Domain.Ports
{
    public interface IProposalRepository
    {
        Task AddAsync(Proposal proposal, CancellationToken cancellationToken = default);
        Task<Proposal?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task UpdateAsync(Proposal proposal, CancellationToken cancellationToken = default);
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
