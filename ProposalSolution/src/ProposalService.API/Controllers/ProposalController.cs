using MediatR;
using Microsoft.AspNetCore.Mvc;
using ProposalService.Application.Commands;

namespace ProposalService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProposalController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProposalController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        //Implementar
        return Ok();
    }

    [HttpPost("Create")]
    public async Task<IActionResult> Create([FromBody] CreateProposalCommand command)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var proposalId = await _mediator.Send(command);

        return Ok(new { Id = proposalId, Message = "Proposta criada com sucesso." });
    }

    [HttpPost("{id:guid}/approve")]
    public async Task<IActionResult> ApproveProposal(Guid id)
    {
        var result = await _mediator.Send(new ApproveProposalCommand(id));

        if (!result)
            return NotFound("Proposta não encontrada.");

        return Ok("Proposta aprovada com sucesso.");
    }
}
