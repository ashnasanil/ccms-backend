using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CCMS.API.Controllers;

[ApiController]
[Route("api/bank/cases")]
[Authorize(Roles = "BankOfficer")]
public class BankResponsesController : ControllerBase
{
    private readonly ISender _sender;

    public BankResponsesController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost("{id:guid}/freeze")]
    public async Task<IActionResult> SubmitFreezeResponse(Guid id, [FromBody] CCMS.Application.Features.Bank.Responses.Commands.SubmitFreezeResponseCommand command)
    {
        if (id != command.CaseId) return BadRequest();
        await _sender.Send(command); 
        return Ok();
    }

    [HttpPost("{id:guid}/balance")]
    public async Task<IActionResult> SubmitBalanceResponse(Guid id, [FromBody] CCMS.Application.Features.Bank.Responses.Commands.SubmitBalanceResponseCommand command)
    {
        if (id != command.CaseId) return BadRequest();
        await _sender.Send(command); 
        return Ok();
    }
}
