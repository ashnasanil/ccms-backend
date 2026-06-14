using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CCMS.Application.DTOs.Court;
using CCMS.Domain.Entities;

namespace CCMS.Application.Interfaces
{
    public interface ICourtService
    {
        Task<CourtDashboardDto> GetDashboardAsync();
        Task<CaseResponseDto> CreateCaseAsync(CreateCaseDto dto);
        Task<IEnumerable<CaseListDto>> GetCasesAsync();
        Task<CaseDetailDto> GetCaseByIdAsync(Guid id);
        
        void ValidateAttachments(Microsoft.AspNetCore.Http.IFormFile courtOrder, Microsoft.AspNetCore.Http.IFormFile aadhaar, Microsoft.AspNetCore.Http.IFormFile pan);
        Task<List<Attachment>> ProcessAttachments(Guid caseId, Microsoft.AspNetCore.Http.IFormFile courtOrder, Microsoft.AspNetCore.Http.IFormFile aadhaar, Microsoft.AspNetCore.Http.IFormFile pan);
    }
}
