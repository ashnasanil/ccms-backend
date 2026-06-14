using System;
using System.Collections.Generic;
using CCMS.Domain.Enums;

namespace CCMS.Application.Features.Bank.Cases.DTOs
{
    public class CaseDetailDto
    {
        public Guid Id { get; set; }
        public string CaseNumber { get; set; } = string.Empty;
        public CaseStatus Status { get; set; }
        public OrderType OrderType { get; set; }
        public string DefendantName { get; set; } = string.Empty;
        public string DefendantAadhaar { get; set; } = string.Empty;
        public string DefendantPAN { get; set; } = string.Empty;
        public string DefendantAccountNumber { get; set; } = string.Empty;
        public string DefendantBankName { get; set; } = string.Empty;
        
        public string MatchedAccountNumber { get; set; } = string.Empty;
        public decimal? MatchedBalance { get; set; }
        public string MatchedAccountStatus { get; set; } = string.Empty;
        
        public List<AttachmentDto> Attachments { get; set; } = new();
        public CaseResponseDto? ExistingResponse { get; set; }
    }

    public class AttachmentDto
    {
        public Guid Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
    }

    public class CaseResponseDto
    {
        public Guid Id { get; set; }
        public ResponseType ResponseType { get; set; }
        public decimal? FreezeAmount { get; set; }
        public decimal? BalanceAmount { get; set; }
        public string Remarks { get; set; } = string.Empty;
        public string RespondedBy { get; set; } = string.Empty;
        public DateTime RespondedAt { get; set; }
    }
}
