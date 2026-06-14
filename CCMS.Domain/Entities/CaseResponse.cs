// FILE: Entities/CaseResponse.cs
using System;
using CCMS.Domain.Common;
using CCMS.Domain.Enums;

namespace CCMS.Domain.Entities
{
    public class CaseResponse : BaseEntity
    {
        public Guid CaseId { get; set; }
        public ResponseType ResponseType { get; set; }
        public decimal? FreezeAmount { get; set; }
        public decimal? BalanceAmount { get; set; }
        public string Remarks { get; set; } = string.Empty;
        public string RespondedBy { get; set; } = string.Empty;
        public DateTime RespondedAt { get; set; }
        
        public Case Case { get; set; } = null!;
    }
}
