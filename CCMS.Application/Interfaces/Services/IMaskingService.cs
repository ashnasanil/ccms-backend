namespace CCMS.Application.Interfaces.Services
{
    public interface IMaskingService
    {
        string MaskAadhaar(string? aadhaar);
        string MaskPAN(string? pan);
        string MaskAccountNumber(string? accNumber);
    }
}
