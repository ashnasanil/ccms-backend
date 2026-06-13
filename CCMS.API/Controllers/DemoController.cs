using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CCMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DemoController : ControllerBase
{
    [HttpGet("court-only")]
    [Authorize(Roles = "Court")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public IActionResult GetCourtData()
    {
        return Ok(new { message = "Access granted. Authorized as a Court Officer." });
    }

    [HttpGet("bank-only")]
    [Authorize(Roles = "Bank")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public IActionResult GetBankData()
    {
        return Ok(new { message = "Access granted. Authorized as a Bank Officer." });
    }
}
