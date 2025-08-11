using MediatR;
namespace ProposalService.Application.Commands;

public record CreateProposalCommand(string Title, decimal Amount) : IRequest<Guid>;
