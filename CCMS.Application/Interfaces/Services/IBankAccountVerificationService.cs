using System.Threading.Tasks;

namespace CCMS.Application.Interfaces.Services
{
    public class BankAccountDetails
    {
        public string AccountNumber { get; set; } = string.Empty;
        public decimal Balance { get; set; }
        public string Status { get; set; } = string.Empty;
        public bool IsMatch { get; set; }
    }

    public interface IBankAccountVerificationService
    {
        Task<BankAccountDetails> VerifyAccountAsync(string? accountNumber, string? aadhaar, string? pan);
    }
}
