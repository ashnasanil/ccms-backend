// FILE: Entities/Case.cs
using System;
using System.Collections.Generic;
using CCMS.Domain.Common;
using CCMS.Domain.Enums;

namespace CCMS.Domain.Entities;

public class Case : BaseEntity
{
    public string CaseNumber { get; set; } = string.Empty;
    public CaseStatus Status { get; set; }
    public OrderType OrderType { get; set; }
    public string DefendantName { get; set; } = string.Empty;
    public decimal FreezeAmount { get; set; }

    public string ComplainantName { get; set; } = string.Empty;
    public string AadhaarNumber { get; set; } = string.Empty;
    public string PanNumber { get; set; } = string.Empty;
    public string AccountNumber { get; set; } = string.Empty;
    public string BankName { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }

    public string? MatchedAccountNumber { get; set; }
    public decimal? MatchedBalance { get; set; }
    public string? MatchedAccountStatus { get; set; }
    
    public ICollection<Attachment> Attachments { get; set; } = new List<Attachment>();
    public CaseResponse? CaseResponse { get; set; }
}
