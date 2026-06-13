using System.Collections.Generic;
using System.Threading.Tasks;
using CCMS.Application.DTOs.Court;

namespace CCMS.Application.Interfaces
{
    public interface ICourtService
    {
        Task<CourtDashboardDto> GetDashboardAsync();
        Task<CaseResponseDto> CreateCaseAsync(CreateCaseDto dto);
        Task<IEnumerable<CaseListDto>> GetCasesAsync();
        Task<CaseDetailDto> GetCaseByIdAsync(int id);
    }
}
