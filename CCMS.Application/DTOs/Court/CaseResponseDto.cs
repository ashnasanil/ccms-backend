using System;
using CCMS.Domain.Enums;

namespace CCMS.Application.DTOs.Court
{
    public class CaseResponseDto
    {
        public Guid Id { get; set; }
        public string CaseNumber { get; set; }
        public CaseStatus Status { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
