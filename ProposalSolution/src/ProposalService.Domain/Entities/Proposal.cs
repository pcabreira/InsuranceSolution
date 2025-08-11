using ProposalService.Domain.Enums;

namespace ProposalService.Domain.Entities;

public sealed class Proposal
{
    public Guid Id { get; private set; }
    public string Title { get; private set; } = null!;
    public decimal Amount { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public ProposalStatus Status { get; private set; }

    private Proposal() { } // EF

    private Proposal(string title, decimal amount)
    {
        if (string.IsNullOrWhiteSpace(title)) throw new ArgumentException("Title required", nameof(title));
        if (amount <= 0) throw new ArgumentException("Amount must be > 0", nameof(amount));

        Id = Guid.NewGuid();
        Title = title;
        Amount = amount;
        CreatedAt = DateTime.UtcNow;
        Status = ProposalStatus.Created;
    }

    public static Proposal Create(string title, decimal amount)
        => new Proposal(title, amount);

    public void ChangeStatus(ProposalStatus newStatus) =>
        Status = newStatus;
}
