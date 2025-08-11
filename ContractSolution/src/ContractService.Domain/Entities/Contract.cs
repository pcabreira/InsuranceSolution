namespace ContractService.Domain.Entities;

public class Contract
{
    public Guid Id { get; private set; }
    public Guid ProposalId { get; private set; }
    public DateTime ContractDate { get; private set; }

    private Contract() { } // EF

    private Contract(Guid proposalId, DateTime contractDate)
    {
        Id = Guid.NewGuid();
        ProposalId = proposalId;
        ContractDate = contractDate;
    }

    public static Contract Create(Guid proposalId, DateTime? contractDate = null)
    {
        if (proposalId == Guid.Empty)
            throw new ArgumentException("Id da proposta obrigatório.", nameof(proposalId));

        var date = contractDate ?? DateTime.UtcNow;
        return new Contract(proposalId, date);
    }
}
