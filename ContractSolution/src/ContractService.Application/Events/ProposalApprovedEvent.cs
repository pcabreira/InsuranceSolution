using ContractService.Domain.Enums;

namespace ContractService.Application.Events;

public record ProposalApprovedEvent(Guid ProposalId, string Status);
