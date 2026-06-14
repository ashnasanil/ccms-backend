using System.Threading.Tasks;
using CCMS.Application.Interfaces.Services;
using CCMS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CCMS.Infrastructure.Services
{
    public class BankAccountVerificationService : IBankAccountVerificationService
    {
        private readonly AppDbContext _context;

        public BankAccountVerificationService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<BankAccountDetails> VerifyAccountAsync(string? accountNumber, string? aadhaar, string? pan)
        {
            var match = await _context.BankCustomers.FirstOrDefaultAsync(c => 
                (!string.IsNullOrEmpty(accountNumber) && c.AccountNumber == accountNumber) ||
                (!string.IsNullOrEmpty(aadhaar) && c.Aadhaar == aadhaar) ||
                (!string.IsNullOrEmpty(pan) && c.PAN == pan)
            );

            if (match != null)
            {
                return new BankAccountDetails
                {
                    IsMatch = true,
                    AccountNumber = match.AccountNumber,
                    Balance = match.Balance,
                    Status = match.AccountStatus
                };
            }

            return new BankAccountDetails { IsMatch = false };
        }
    }
}
