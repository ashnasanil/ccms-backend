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
        Task<CaseDetailDto> GetCaseByCaseNumberAsync(string caseNumber);
        
        void ValidateAttachments(Microsoft.AspNetCore.Http.IFormFile courtOrder, Microsoft.AspNetCore.Http.IFormFile aadhaar, Microsoft.AspNetCore.Http.IFormFile pan);
        Task<List<AttachmentDto>> ProcessAttachments(Microsoft.AspNetCore.Http.IFormFile courtOrder, Microsoft.AspNetCore.Http.IFormFile aadhaar, Microsoft.AspNetCore.Http.IFormFile pan);
    }
}
