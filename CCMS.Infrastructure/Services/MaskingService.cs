using CCMS.Application.Interfaces.Services;

namespace CCMS.Infrastructure.Services
{
    public class MaskingService : IMaskingService
    {
        public string MaskAadhaar(string? aadhaar)
        {
            if (string.IsNullOrWhiteSpace(aadhaar) || aadhaar.Length < 4) return aadhaar ?? string.Empty;
            return "********" + aadhaar.Substring(aadhaar.Length - 4);
        }

        public string MaskPAN(string? pan)
        {
            if (string.IsNullOrWhiteSpace(pan) || pan.Length < 4) return pan ?? string.Empty;
            return "******" + pan.Substring(pan.Length - 4);
        }

        public string MaskAccountNumber(string? accNumber)
        {
            if (string.IsNullOrWhiteSpace(accNumber) || accNumber.Length < 4) return accNumber ?? string.Empty;
            return "******" + accNumber.Substring(accNumber.Length - 4);
        }
    }
}
