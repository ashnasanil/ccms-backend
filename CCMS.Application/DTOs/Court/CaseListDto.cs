using System;

namespace CCMS.Application.DTOs.Court
{
    public class CaseListDto
    {
        public string CaseNumber { get; set; }
        public string DefendantName { get; set; }
        public string OrderType { get; set; }
        public string Status { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
