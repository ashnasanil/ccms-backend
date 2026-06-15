using System;

namespace CCMS.Application.Features.Bank.Responses.DTOs
{
    public class FreezeResponseDto
    {
        public Guid CaseId { get; set; }
        public decimal FreezeAmount { get; set; }
        public string Remarks { get; set; } = string.Empty;
    }
}
