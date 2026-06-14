// FILE: Entities/BankCustomer.cs
using System;
using CCMS.Domain.Common;

namespace CCMS.Domain.Entities
{
    public class BankCustomer : BaseEntity
    {
        public string AccountNumber { get; set; } = string.Empty;
        public string Aadhaar { get; set; } = string.Empty;
        public string PAN { get; set; } = string.Empty;
        public decimal Balance { get; set; }
        public string AccountStatus { get; set; } = string.Empty;
        public string BankName { get; set; } = string.Empty;
    }
}
