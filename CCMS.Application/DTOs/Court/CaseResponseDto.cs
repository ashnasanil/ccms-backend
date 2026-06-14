using System;
using CCMS.Domain.Enums;

namespace CCMS.Application.DTOs.Court
{
    public class CaseResponseDto
    {
        public string CaseNumber { get; set; }
        public CaseStatus Status { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
