using ContractService.Application.Commands;
using ContractService.Application.Ports;
using ContractService.Domain.Entities;
using MediatR;

namespace ContractService.Application.Handlers
{
    public class CreateContractCommandHandler : IRequestHandler<CreateContractCommand, Guid>
    {
        private readonly IContractRepository _repository;

        public CreateContractCommandHandler(IContractRepository repository)
        {
            _repository = repository;
        }

        public async Task<Guid> Handle(CreateContractCommand request, CancellationToken cancellationToken)
        {
            var existingContract = await _repository.GetByProposalIdAsync(request.ProposalId, cancellationToken);

            if (existingContract != null)
                throw new InvalidOperationException($"Contrato já existe para a proposta {request.ProposalId}");

            var contract = Contract.Create(request.ProposalId, request.ContractDate);

            await _repository.AddAsync(contract, cancellationToken);

            return contract.Id;
        }
    }
}
