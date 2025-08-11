using MediatR;
using Microsoft.Extensions.Configuration;
using ProposalService.Application.Commands;
using ProposalService.Application.Ports;
using ProposalService.Domain.Enums;
using ProposalService.Domain.Ports;

public class ApproveProposalCommandHandler : IRequestHandler<ApproveProposalCommand, bool>
{
    private readonly IProposalRepository _repository;
    private readonly IEventPublisher _publisher;
    private readonly string _routingKey;

    public ApproveProposalCommandHandler(IProposalRepository repository, 
                                         IEventPublisher publisher, 
                                         IConfiguration configuration)
    {
        _repository = repository;
        _publisher = publisher;

        var section = configuration.GetSection("RabbitMq");
        _routingKey = section["Queues:StatusApproved:RoutingKey"] ?? "proposal.approved";
    }

    public async Task<bool> Handle(ApproveProposalCommand request, CancellationToken cancellationToken)
    {
        var proposal = await _repository.GetByIdAsync(request.ProposalId);
        if (proposal == null)
            return false;

        proposal.ChangeStatus(ProposalStatus.Approved);
        await _repository.UpdateAsync(proposal);

        // Publica a mensagem informando que a proposta foi aprovada
        await _publisher.PublishAsync(_routingKey, new
        {
            ProposalId = proposal.Id,
            Status = proposal.Status.ToString()
        }, cancellationToken);

        return true;
    }
}

