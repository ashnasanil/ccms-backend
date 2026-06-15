using System;
using CCMS.Domain.Common;

namespace CCMS.Domain.Entities;

public class BankCustomer : BaseEntity
{
    public string AccountNumber { get; set; } = string.Empty;
    public string AadhaarNumber { get; set; } = string.Empty;
    public string PanNumber { get; set; } = string.Empty;
    public decimal Balance { get; set; }
    public string AccountStatus { get; set; } = "Active";
    public string BankName { get; set; } = string.Empty;
}
