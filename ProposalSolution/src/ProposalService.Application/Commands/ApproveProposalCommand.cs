using MediatR;

namespace ProposalService.Application.Commands
{
    public record ApproveProposalCommand(Guid ProposalId) : IRequest<bool>;
}
