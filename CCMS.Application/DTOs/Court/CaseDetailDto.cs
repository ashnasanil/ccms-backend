using System;
using System.Collections.Generic;

namespace CCMS.Application.DTOs.Court
{
    public class CaseDetailDto
    {
        public string CaseNumber { get; set; }
        public string ComplainantName { get; set; }
        public string DefendantName { get; set; }
        public string AadhaarNumber { get; set; }
        public string PanNumber { get; set; }
        public string AccountNumber { get; set; }
        public string BankName { get; set; }
        public string OrderType { get; set; }
        public decimal? FreezeAmount { get; set; }
        public string Status { get; set; }
        public DateTime CreatedDate { get; set; }
        
        public List<AttachmentDto> Attachments { get; set; }
        public object BankResponse { get; set; }
    }
}
