namespace ContractService.Application.Commands;

public class CreateContractRequest
{
    public Guid ProposalId { get; }
    public DateTime ContractDate { get; }

    public CreateContractRequest(Guid proposalId, DateTime contractDate)
    {
        ProposalId = proposalId;
        ContractDate = contractDate;
    }
}
