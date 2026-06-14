using System;
using CCMS.Domain.Enums;

namespace CCMS.Application.Features.Bank.Cases.DTOs
{
    public class CaseListDto
    {
        public Guid Id { get; set; }
        public string CaseNumber { get; set; } = string.Empty;
        public CaseStatus Status { get; set; }
        public OrderType OrderType { get; set; }
        public string DefendantName { get; set; } = string.Empty;
    }
}
