using System;

namespace CCMS.Application.Features.Bank.Responses.DTOs
{
    public class BalanceResponseDto
    {
        public Guid CaseId { get; set; }
        public decimal BalanceAmount { get; set; }
        public string Remarks { get; set; } = string.Empty;
    }
}
