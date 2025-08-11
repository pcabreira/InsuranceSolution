using ContractService.Application.Commands;
using ContractService.Application.Events;
using ContractService.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace ContractService.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ContractsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IConfiguration _configuration;

    public ContractsController(IMediator mediator, IConfiguration configuration)
    {
        _mediator = mediator;
        _configuration = configuration;
    }

    [HttpPost("Create")]
    public async Task<IActionResult> Create()
    {
        var factory = new ConnectionFactory
        {
            HostName = _configuration["RabbitMQ:HostName"] ?? "localhost"
        };

        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();

        var queueName = _configuration["RabbitMQ:QueueName"] ?? "proposals.approved";

        var result = channel.BasicGet(queueName, autoAck: false);

        if (result == null)
            return BadRequest("Nenhuma mensagem pendente na fila para processar.");

        var message = Encoding.UTF8.GetString(result.Body.ToArray());

        ProposalApprovedEvent? proposalEvent;

        try
        {
            proposalEvent = JsonSerializer.Deserialize<ProposalApprovedEvent>(message);
        }
        catch
        {
            channel.BasicReject(result.DeliveryTag, false);
            return BadRequest("Mensagem inválida na fila.");
        }

        if (proposalEvent == null || proposalEvent.Status != ProposalStatus.Approved.ToString())
        {
            channel.BasicAck(result.DeliveryTag, false);
            return BadRequest("Proposta não aprovada ou inválida.");
        }

        var contractId = await _mediator.Send(new CreateContractCommand(proposalEvent.ProposalId, DateTime.UtcNow));

        channel.BasicAck(result.DeliveryTag, false);

        return Created($"/api/contracts/{contractId}", new { id = contractId });
    }
}

