using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CCMS.API.Controllers;

[ApiController]
[Route("api/batch")]
[Authorize(Roles = "CourtOfficer")]
public class BatchController : ControllerBase
{
    private readonly ISender _sender;

    public BatchController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost("run")]
    public async Task<IActionResult> RunBatch()
    {
        await _sender.Send(new CCMS.Application.Features.Bank.Batch.Commands.TriggerBatchCommand()); 
        return Ok();
    }

    [HttpGet("logs")]
    public async Task<IActionResult> GetLogs()
    {
        // Temporarily return empty until GetBatchLogsQuery is implemented
        return Ok(new object[] { });
    }
}
