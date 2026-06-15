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

    public record SubmitFreezeResponseDto(decimal FreezeAmount, string Remarks);
    public record SubmitBalanceResponseDto(decimal BalanceAmount, string Remarks);

    [HttpPost("{id:guid}/freeze")]
    public async Task<IActionResult> SubmitFreezeResponse(Guid id, [FromBody] SubmitFreezeResponseDto dto)
    {
        var command = new CCMS.Application.Features.Bank.Responses.Commands.SubmitFreezeResponseCommand(id, dto.FreezeAmount, dto.Remarks);
        await _sender.Send(command); 
        return Ok();
    }

    [HttpPost("{id:guid}/balance")]
    public async Task<IActionResult> SubmitBalanceResponse(Guid id, [FromBody] SubmitBalanceResponseDto dto)
    {
        var command = new CCMS.Application.Features.Bank.Responses.Commands.SubmitBalanceResponseCommand(id, dto.BalanceAmount, dto.Remarks);
        await _sender.Send(command); 
        return Ok();
    }
}
