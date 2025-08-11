using ContractService.Domain.Entities;

namespace ContractService.Application.Ports;

public interface IContractRepository
{
    Task<Contract?> GetByProposalIdAsync(Guid proposalId, CancellationToken cancellationToken);
    Task AddAsync(Contract contract, CancellationToken cancellationToken);
}
