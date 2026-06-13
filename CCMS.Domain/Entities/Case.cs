using System;
using CCMS.Domain.Enums;

namespace CCMS.Domain.Entities
{
    public class Case
    {
        public int Id { get; set; }
        public string CaseNumber { get; set; }
        public string ComplainantName { get; set; }
        public string DefendantName { get; set; }
        public string AadhaarNumber { get; set; }
        public string PanNumber { get; set; }
        public string AccountNumber { get; set; }
        public string BankName { get; set; }
        public OrderType OrderType { get; set; }
        public decimal? FreezeAmount { get; set; }
        public CaseStatus Status { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
