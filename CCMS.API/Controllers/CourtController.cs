using System.Threading.Tasks;
using CCMS.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CCMS.API.Controllers
{
    [ApiController]
    [Route("api/court")]
    public class CourtController : ControllerBase
    {
        private readonly ICourtService _courtService;

        public CourtController(ICourtService courtService)
        {
            _courtService = courtService;
        }

        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboardAsync()
        {
            var result = await _courtService.GetDashboardAsync();
            return Ok(result);
        }
    }
}
