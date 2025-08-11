using MediatR;
using ProposalService.Application.Commands;
using ProposalService.Domain.Entities;
using ProposalService.Domain.Ports;

namespace ProposalService.Application.Handlers
{
    public class CreateProposalHandler : IRequestHandler<CreateProposalCommand, Guid>
    {
        private readonly IProposalRepository _repo;

        public CreateProposalHandler(IProposalRepository repo)
        {
            _repo = repo;
        }

        public async Task<Guid> Handle(CreateProposalCommand request, CancellationToken cancellationToken)
        {
            var proposal = Proposal.Create(request.Title, request.Amount);
            await _repo.AddAsync(proposal, cancellationToken);
            await _repo.SaveChangesAsync(cancellationToken);

            return proposal.Id;
        }
    }
}

