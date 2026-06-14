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

        [HttpGet("cases")]
        public async Task<IActionResult> GetCasesAsync()
        {
            var result = await _courtService.GetCasesAsync();
            return Ok(result);
        }

        [HttpGet("cases/{id}")]
        public async Task<IActionResult> GetCaseByIdAsync(int id)
        {
            var result = await _courtService.GetCaseByIdAsync(id);
            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpPost("cases")]
        public async Task<IActionResult> CreateCaseAsync([FromForm] CCMS.Application.DTOs.Court.CreateCaseDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var result = await _courtService.CreateCaseAsync(dto);
                return Ok(result);
            }
            catch (System.ArgumentException ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }
    }
}
