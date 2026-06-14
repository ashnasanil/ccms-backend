using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CCMS.API.Controllers;

[ApiController]
[Route("api/bank/cases")]
[Authorize(Roles = "BankOfficer")]
public class BankCasesController : ControllerBase
{
    private readonly ISender _sender;

    public BankCasesController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    public async Task<IActionResult> GetCases([FromQuery] string statusCategory = "Pending")
    {
        var result = await _sender.Send(new CCMS.Application.Features.Bank.Cases.Queries.GetCasesByStatusQuery(statusCategory)); 
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetCase(Guid id)
    {
        var result = await _sender.Send(new CCMS.Application.Features.Bank.Cases.Queries.GetCaseDetailQuery(id)); 
        return Ok(result);
    }
}
