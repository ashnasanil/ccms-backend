using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CCMS.API.Controllers;

[ApiController]
[Route("api/bank/dashboard")]
[Authorize(Roles = "BankOfficer")]
public class BankDashboardController : ControllerBase
{
    private readonly ISender _sender;

    public BankDashboardController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    public async Task<IActionResult> GetDashboard()
    {
        var result = await _sender.Send(new CCMS.Application.Features.Bank.Dashboard.Queries.GetDashboardSummaryQuery()); 
        return Ok(result);
    }
}
