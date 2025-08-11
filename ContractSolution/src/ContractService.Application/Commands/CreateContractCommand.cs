using MediatR;

namespace ContractService.Application.Commands;

public record CreateContractCommand(Guid ProposalId, DateTime? ContractDate) : IRequest<Guid>;
